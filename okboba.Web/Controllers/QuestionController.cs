﻿using Microsoft.AspNet.Identity;
using okboba.Entities;
using okboba.Repository;
using okboba.Repository.EntityRepository;
using okboba.Repository.Models;
using okboba.Repository.WebClient;
using okboba.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace okboba.Controllers
{
    [Authorize]
    public class QuestionController : OkbBaseController
    {
        private IProfileRepository _profileRepo;
        private IQuestionRepository _quesRepo;
        private MatchApiClient _matchApiClient;

        public QuestionController()
        {
            _profileRepo = EntityProfileRepository.Instance;
            _quesRepo = EntityQuestionRepository.Instance;
        }

        // GET: Question
        public ActionResult Index(string userId, int page = 1)
        {
            var vm = new QuestionIndexViewModel();

            var me = GetProfileId();

            if (string.IsNullOrEmpty(userId) || userId == User.Identity.GetUserId())
            {
                //Viewing own questions
                vm.UserId = "";
                vm.ProfileId = me;
                vm.IsMe = true;
                vm.Questions = _quesRepo.GetQuestions(me, page);
                vm.NextQuestions = _quesRepo.Next2Questions(me);
            }
            else
            {
                //Viewing other person's questions
                var id = _profileRepo.GetProfileId(userId);

                vm.UserId = userId;
                vm.ProfileId = id;
                vm.IsMe = false;
                vm.Questions = _quesRepo.GetQuestions(id, page);

                //Get my own questions for comparison - just need to get the answers from the match API cache
                //Convert to dictionary for easier comparison
                vm.CompareQuestions = _quesRepo.GetAnswers(me);

            }

            return View(vm);
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
        /// operations in a transaction. If getNextFlag = false we don't need to get the next
        /// question, otherwise return the next two questions as a JSON object.  We have all
        /// this logic in the controller instead of repository because the web client requires
        /// an authentication cookie which is stored in the HttpContext object.
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

            //Convert user input to Answer object for repository
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

            //Wrap the operations in a transaction
            //By default if scope.Complete() isn't called the transaction is rolled back.
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options, TransactionScopeAsyncFlowOption.Enabled))
            {
                Task t1, t2;
                t1 = _quesRepo.AnswerAsync(answer);
                t2 = _matchApiClient.AnswerAsync(answer);

                await Task.WhenAll(t1, t2);

                scope.Complete();
            }

            IList<QuestionModel> nextQuestions = null;

            if (getNextFlag)
            {
                nextQuestions = _quesRepo.Next2Questions(profileId);
            }

            return Json(nextQuestions);
        }
    }
}