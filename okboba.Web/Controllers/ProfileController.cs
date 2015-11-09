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
using okboba.Repository;
using okboba.Repository.EntityRepository;

namespace okboba.Controllers
{

    [Authorize]
    public class ProfileController : OkbBaseController
    {
        private IProfileRepository _profileRepo;

        public ProfileController()
        {
            _profileRepo = EntityProfileRepository.Instance;
        }

        /// <summary>
        /// View your own profile
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var profileId = GetProfileId();
            var profile = _profileRepo.GetProfile(profileId);

            var vm = new ProfileViewModel(profile);

            vm.ProfileText = profile.ProfileText;

            //It's possible they don't have any profile text..return empty object
            if (vm.ProfileText == null)
            {
                vm.ProfileText = new ProfileText();
            }

            return View(vm);
        }

        /// <summary>
        /// API: Update profile text
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditProfileText(string qText, string whichQuestion)
        {
            //Massage the input
            qText = HttpContext.Server.HtmlEncode(qText);
            qText = Truncate(qText, 4000);

            var db = new OkbDbContext();

            //Check if we are adding or updating
            var currProfileText = db.ProfileTexts.Find(GetProfileId());

            if(currProfileText==null)
            {
                currProfileText = new ProfileText { ProfileId = GetProfileId() };
                db.ProfileTexts.Add(currProfileText);
            }
            
            switch (whichQuestion)
            {
                case "q1":
                    currProfileText.Question1 = qText;
                    break;
                case "q2":
                    currProfileText.Question2 = qText;
                    break;
                case "q3":
                    currProfileText.Question3 = qText;
                    break;
                case "q4":
                    currProfileText.Question4 = qText;
                    break;
                case "q5":
                    currProfileText.Question5 = qText;
                    break;
                default:
                    Response.StatusCode = 400; //Client error
                    break;
            }

            db.SaveChanges();

            return Content("{}");
        }
    }
}