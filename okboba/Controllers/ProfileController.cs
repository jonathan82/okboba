using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using okboba.Web.Models;
using System.Data.Entity.Migrations;
using System.Linq.Expressions;

namespace okboba.Controllers
{
    [Authorize]
    public class ProfileController : OkbBaseController
    {
        // GET: Profile
        // Show my own Profile
        public ActionResult Index()
        {
            var profile = GetUserProfile();

            var vm = new ProfileViewModel { Profile = profile };

            vm.ProfileText = profile.ProfileText;

            //It's possible they don't have any profile text..return empty object
            if (vm.ProfileText == null)
            {
                vm.ProfileText = new ProfileText();
            }

            //Return the view model
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditProfileText(string qText, string whichQuestion)
        {
            var p = new ProfileText();

            p.ProfileId = GetProfileId();

            Expression<Func<ProfileText, object>> whichQuesExpr = null;

            switch (whichQuestion)
            {
                case "q1":
                    p.Question1 = qText;
                    whichQuesExpr = q => q.Question1;
                    break;
                case "q2":
                    p.Question2 = qText;
                    whichQuesExpr = q => q.Question2;
                    break;
                case "q3":
                    p.Question3 = qText;
                    whichQuesExpr = q => q.Question3;
                    break;
                case "q4":
                    p.Question4 = qText;
                    whichQuesExpr = q => q.Question4;
                    break;
                case "q5":
                    p.Question5 = qText;
                    whichQuesExpr = q => q.Question5;
                    break;
                default:
                    Response.StatusCode = 400; //Client error
                    break;
            }


            OkbDbContext db = new OkbDbContext();

            db.ProfileTexts.AddOrUpdate(whichQuesExpr, p);
            db.SaveChanges();

            return Content("{}");
        }
    }
}