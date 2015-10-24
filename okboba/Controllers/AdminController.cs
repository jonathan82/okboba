using okboba.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace okboba.Controllers
{
    public class AdminController : OkbBaseController
    {
        public QuestionRepository quesRepo { get; set; }

        public AdminController()
        {
            this.quesRepo = QuestionRepository.Instance;
        }

        public JsonResult GetTranslateQuestions()
        {
            var list = quesRepo.GetTranslateQuestions();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin
        public ActionResult Questions()
        {
            return View();
        }
    }
}