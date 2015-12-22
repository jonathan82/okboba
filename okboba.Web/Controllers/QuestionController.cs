using Microsoft.AspNet.Identity;
using okboba.Entities;
using okboba.Repository;
using okboba.Repository.EntityRepository;
using okboba.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace okboba.Controllers
{
    [Authorize]
    public class QuestionController : OkbBaseController
    {
        private IProfileRepository _profileRepo;
        private IQuestionRepository _quesRepo;

        public QuestionController()
        {
            this._profileRepo = EntityProfileRepository.Instance;
            this._quesRepo = EntityQuestionRepository.Instance;
        }

        // GET: Question
        public ActionResult Index(string userId)
        {
            var vm = new QuestionIndexViewModel();

            var me = GetProfileId();

            if (string.IsNullOrEmpty(userId) || userId == User.Identity.GetUserId())
            {
                //Viewing own questions
                vm.ProfileId = me;
                vm.IsMe = true;
                vm.Questions = _quesRepo.GetQuestions(me);
                vm.NextQuestions = _quesRepo.Next2Questions(me);
            }
            else
            {
                //Viewing other person's questions
                var id = _profileRepo.GetProfileId(userId);

                vm.ProfileId = id;
                vm.IsMe = false;
                vm.Questions = _quesRepo.GetQuestions(id);

                //Get my own questions for comparison
                //Convert to dictionary for easier comparison
                var compareQuestions = _quesRepo.GetQuestions(id);
                vm.CompareQuestions = new Dictionary<short, Answer>();
                foreach (var q in compareQuestions)
                {
                    vm.CompareQuestions.Add(q.Question.Id, q.Answer);
                }
            }

            return View(vm);
        }

        /// <summary>
        /// Answers or updates their answer then returns a JSON object for the 
        /// next 2 questions. If updateFlag is true then no need to get the next
        /// 2 questions.
        /// </summary>
        public JsonResult Answer(AnswerViewModel input, bool updateFlag = false, bool skipFlag = false)
        {
            var profileId = GetProfileId();

            var answer = new Answer
            {
                ProfileId = profileId,
                QuestionId = (short)input.QuestionId
            };

            if (skipFlag)
            {
                //skipping question               
                answer.ChoiceIndex = 0;
                answer.ChoiceAccept = 0;
                answer.ChoiceWeight = 0;
            }
            else
            {
                //Convert user input to Answer object for repository
                byte acceptBits = 0;
                foreach (var index in input.ChoiceAccept)
                {
                    acceptBits |= (byte)(1 << (index - 1));
                }

                answer.ChoiceIndex = (byte)input.ChoiceIndex;
                answer.ChoiceAccept = acceptBits;
                answer.ChoiceWeight = (byte)input.ChoiceImportance;                
            }

            _quesRepo.Answer(answer);

            var nextQuestions = _quesRepo.Next2Questions(profileId);

            return Json(nextQuestions);
        }
    }
}