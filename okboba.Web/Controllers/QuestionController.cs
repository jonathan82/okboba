using Microsoft.AspNet.Identity;
using okboba.Entities;
using okboba.Repository;
using okboba.Repository.EntityRepository;
using okboba.Repository.Models;
using okboba.Repository.WebClient;
using okboba.Resources;
using okboba.Web.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace okboba.Web.Controllers
{
    [Authorize]
    public class QuestionController : OkbBaseController
    {
        private IProfileRepository _profileRepo;
        private IQuestionRepository _quesRepo;
        private IActivityRepository _feedRepo;

        private MatchApiClient _matchApiClient;

        public QuestionController()
        {
            _profileRepo = EntityProfileRepository.Instance;
            _quesRepo = EntityQuestionRepository.Instance;
            _feedRepo = EntityActivityRepository.Instance;
        }

        private int HighestMatchPercent(int count)
        {
            if (count == 0 || count == 1) return 0;
            return (int)((1 - 1 / (float)count) * 100);
        }

        // GET: Question
        public async Task<ActionResult> Index(string userId, int page = 1)
        {
            var webClient = GetMatchApiClient();

            var vm = new QuestionIndexViewModel();

            var me = GetProfileId();

            if (string.IsNullOrEmpty(userId) || userId == User.Identity.GetUserId())
            {
                //Viewing own questions
                vm.UserId = "";
                vm.ProfileId = me;
                vm.IsMe = true;
                vm.Questions = _quesRepo.GetQuestions(me, page, OkbConstants.NUM_QUES_PER_PAGE);
                vm.NextQuestions = _quesRepo.Next2Questions(me);
                vm.HighestMatchPercent = HighestMatchPercent(vm.Questions.TotalItemCount);
            }
            else
            {
                //Viewing other person's questions
                var id = _profileRepo.GetProfileId(userId);

                vm.UserId = userId;
                vm.ProfileId = id;
                vm.IsMe = false;
                vm.Questions = _quesRepo.GetQuestions(id, page, OkbConstants.NUM_QUES_PER_PAGE);
                vm.Profile = _profileRepo.GetProfile(id);
                vm.CompareProfile = _profileRepo.GetProfile(me);

                //Get my own questions for comparison - just need to get the answers from the match API cache                
                //vm.CompareQuestions = _quesRepo.GetAnswers(me);
                vm.CompareQuestions = await webClient.GetIntersectionAsync(me, vm.ProfileId);
            }

            return vm.IsMe ? View("IndexMe", vm) : View(vm);
        }

        /// <summary>
        /// Skip the question. Returns the next 2 questions like Answer but they aren't shown
        /// asynchronously - make the user wait for skipping questions. Doesn't update the 
        /// answer cache.
        /// </summary>
        public async Task<JsonResult> Skip(AnswerViewModel input)
        {
            var profileId = GetProfileId();

            var answer = new Answer
            {
                ProfileId = profileId,
                QuestionId = (short)input.QuestionId,
                ChoiceIndex = null,
                ChoiceAccept = 0,
                ChoiceWeight = 0,
            };

            await _quesRepo.AnswerAsync(answer);

            var nextQuestions = _quesRepo.Next2Questions(profileId);

            return Json(nextQuestions);
        }

        /// <summary>
        /// Add/Updates a user's answer in the database, and then calls MatchApiClient to
        /// update the cache. Responsible for keeping DB and cache in sync so wraps these
        /// operations in a transaction. 
        /// 
        ///   - If getNextFlag = true get the next 2 questions and return them as JSON
        ///   - Otherwise return the user's validated answer
        /// 
        /// More info about TransactionScope (limitations):
        /// https://msdn.microsoft.com/en-us/data/dn456843.aspx
        /// 
        /// </summary>
        public async Task<JsonResult> Answer(AnswerViewModel input, bool getNextFlag = true)
        {
            var profileId = GetProfileId();

            _matchApiClient = GetMatchApiClient();

            var answer = new Answer
            {
                ProfileId = profileId,
                QuestionId = (short)input.QuestionId
            };

            //Convert checkboxes to bit array for Acceptable choices
            byte acceptBits = 0;

            foreach (var index in input.ChoiceAccept)
            {
                acceptBits |= (byte)(1 << (index - 1));
            }

            answer.ChoiceIndex = (byte)input.ChoiceIndex;
            answer.ChoiceAccept = acceptBits;
            answer.ChoiceWeight = (byte)input.ChoiceImportance;

            //We should do some user input validation
            if(!_quesRepo.ValidateAnswer(answer))
            {
                //invalid answer - return not OK
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(null);
            }
            
            // Isolation Level: Read Committed
            // Timeout        : 5 minutes
            var options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(5)
            };

            //Update DB and Cache in a transaction
            //By default if scope.Complete() isn't called the transaction is rolled back.
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options, TransactionScopeAsyncFlowOption.Enabled))
            {
                Task t1, t2;
                t1 = _quesRepo.AnswerAsync(answer);
                t2 = _matchApiClient.AnswerAsync(answer);

                await Task.WhenAll(t1, t2);

                scope.Complete();
            }

            //Update the Activity feed
            if(IsOkToAddActivity(OkbConstants.ActivityCategories.AnsweredQuestion))
            {
                //Get the question text
                var ques = _quesRepo.GetQuestion(input.QuestionId);
                var choiceText = "";                
                choiceText = ques.Choices[((int)answer.ChoiceIndex - 1) % ques.Choices.Count];               
                _feedRepo.AnsweredQuestionActivity(profileId, ques.Text, choiceText);
                UpdateActivityLastAdded(OkbConstants.ActivityCategories.AnsweredQuestion);
            }

            IList<QuestionModel> nextQuestions = null;

            //Get the next questions
            if (getNextFlag)
            {
                nextQuestions = _quesRepo.Next2Questions(profileId);
                return Json(nextQuestions);
            }
            else
            {
                return Json(answer);
            }            
        }
    }
}