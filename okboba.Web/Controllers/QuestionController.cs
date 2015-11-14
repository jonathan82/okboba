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
        public ActionResult Index()
        {
            //var profileId = GetProfileId();
            //var profile = _profileRepo.GetProfile(profileId);
            //var vm = new ProfileViewModel(profile);

            //if(profile.CurrentQuestionId == null)
            //{
            //    // We finished answering all the questions.  
            //    // See if there are any unanswered questions left.
            //}
            //else
            //{
            //    //Get the current question and return it in the view model
            //    vm.CurrentQuestion = _quesRepo.GetQuestion(profile.CurrentQuestionId);
            //}

            //return View(vm);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add an answer to a question for a user and then returns a JSON object for the 
        /// next question.
        /// </summary>
        public JsonResult AnswerQuestion(int questionId, int answerIndex, bool[] answerAccept, int weight,int rank)
        {
            var profileId = GetProfileId();
            
            var nextQues = _quesRepo.GetQuestionByRank(rank + 1);

            //quesRepo.AnswerQuestion(profileId, questionId, answerIndex, answerAccept, weight, nextQues.Id);

            return Json(nextQues);
        }
    }
}