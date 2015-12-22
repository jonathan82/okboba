using okboba.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using okboba.Repository.Models;
using okboba.Repository.EntityRepository;

namespace okboba.Controllers
{
    public class AdminController : OkbBaseController
    {
        //public IQuestionRepository _quesRepo { get; set; }

        //public AdminController()
        //{
        //    this._quesRepo = EntityQuestionRepository.Instance;
        //}

        //public JsonResult GetTranslateQuestions(int page = 1, int pageSize = 25)
        //{
        //    var paged = _quesRepo.GetTranslateQuestions().ToPagedList(page, pageSize);
        //    var quesList = new List<TranslateQuestionViewModel>();

        //    foreach (var q in paged)
        //    {
        //        quesList.Add(new TranslateQuestionViewModel
        //        {
        //            Id = q.Id,
        //            QuesEng = q.QuesEng,
        //            QuesChin = q.QuesChin,
        //            ChoicesEng = q.ChoicesInternalEng == null ? null : q.ChoicesInternalEng.Split(';'),
        //            ChoicesChin = q.ChoicesInternalChin == null ? null : q.ChoicesInternalChin.Split(';'),
        //            Rank = q.Rank,
        //            TraitId = q.TraitId,
        //            Scores = (sbyte[])(Array)q.TraitScores
        //        });
        //    }

        //    return Json(new {Questions = quesList, PageCount = paged.PageCount }, JsonRequestBehavior.AllowGet);
        //}

        //// GET: Admin
        //public ActionResult Questions()
        //{
        //    return View();
        //}
    }
}