using okboba.Repository;
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
        private ProfileRepository profileRepo;
        private QuestionRepository quesRepo;

        public QuestionController()
        {
            this.profileRepo = ProfileRepository.Instance;
            this.quesRepo = QuestionRepository.Instance;
        }

        // GET: Question
        public ActionResult Index()
        {
            var profileId = GetProfileId();
            var profile = profileRepo.GetProfile(profileId);
            var vm = new ProfileViewModel(profile);

            if(profile.CurrentQuestionId == null)
            {
                // We finished answering all the questions.  
                // See if there are any unanswered questions left.
            }
            else
            {
                //Get the current question and return it in the view model
                vm.CurrentQuestion = quesRepo.GetQuestion(profile.CurrentQuestionId);
            }
           
            return View(vm);
        }

        /// <summary>
        /// Add an answer to a question for a user and then returns a JSON object for the 
        /// next question.
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="answerIndex"></param>
        /// <param name="answerAccept"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public JsonResult AnswerQuestion(int questionId, int answerIndex, bool[] answerAccept, int weight,int rank)
        {
            var profileId = GetProfileId();
            
            var nextQues = quesRepo.GetQuestionByRank(rank + 1);

            //quesRepo.AnswerQuestion(profileId, questionId, answerIndex, answerAccept, weight, nextQues.Id);

            return Json(nextQues);
        }
    }
}