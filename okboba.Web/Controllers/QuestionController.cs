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
        public ActionResult Index(int? id)
        {
            var vm = new QuestionIndexViewModel();

            if (id == null)
            {
                vm.ProfileId = GetProfileId();
                vm.IsMe = true;
            }
            else
            {
                vm.ProfileId = (int)id;
                vm.IsMe = false;
            }

            vm.MyQuestions = _quesRepo.GetAnsweredQuestions(vm.ProfileId);
            vm.NextQuestions = _quesRepo.GetNext2Questions(vm.ProfileId);

            return View(vm);
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