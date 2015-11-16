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
using okboba.Repository.WebClient;
using System.Threading.Tasks;

namespace okboba.Controllers
{

    [Authorize]
    public class ProfileController : OkbBaseController
    {
        private IProfileRepository _profileRepo;
        private ILocationRepository _locRepo;
        private MatchApiClient _webClient;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //base.OnActionExecuting(filterContext);
            _webClient = GetMatchApiClient();
        }

        public ProfileController()
        {
            _profileRepo = EntityProfileRepository.Instance;
            _locRepo = EntityLocationRepository.Instance;
        }

        [ChildActionOnly]
        public ActionResult ProfileDetailAndText(int profileId, bool isMe)
        {
            // Get the profile text
            var profileText = _profileRepo.GetProfileText(profileId);

            // Get the profile details as a dictionary
            var profileDetail = _profileRepo.GetProfileDetail(profileId);
            var detailDict = new Dictionary<string, string>();
            foreach (var prop in profileDetail.GetType().GetProperties())
            {
                if (prop.PropertyType != typeof(byte)) continue;
                var val = _profileRepo.GetOptionValue(prop.Name, (byte)(prop.GetValue(profileDetail)));
                detailDict.Add(prop.Name, val);
            }

            //Populate view model
            var vm = new ProfileDetailViewModel
            {
                ProfileText = profileText,
                ProfileDetail = profileDetail,
                DetailDict = detailDict,                
                isMe = isMe
            };

            return PartialView("_ProfileDetailAndText",vm);
        }

        [ChildActionOnly]
        public ActionResult ProfileHeader(int profileId, bool isMe, string section)
        {
            // Get profile info
            var profile = _profileRepo.GetProfile(profileId);

            // Calculate match between users
            var match = _webClient.CalculateMatchAsync(profileId).Result;

            var vm = new ProfileHeaderViewModel
            {
                Match = match,
                Profile = profile,
                IsMe = isMe,
                Location = _locRepo.GetLocationString(profile.LocationId1, profile.LocationId2),
                Section = section
            };

            return PartialView("_ProfileHeader", vm);
        }

        public ActionResult Index(int? profileId)
        {
            var me = GetProfileId();
            var vm = new ProfileViewModel();

            if (profileId == null || profileId == me)
            {
                //Viewing own profile
                vm.ProfileId = me;
                vm.IsMe = true;
            }
            else
            {
                vm.ProfileId = (int)profileId;
                vm.IsMe = false;
            }
           
            return View(vm);
        }

        ///// <summary>
        ///// Return profile page.  Show editing options if viewing own profile.
        ///// </summary>
        //public async Task<ActionResult> Index(int? profileId)
        //{
        //    bool ownProfile = false;

        //    if (profileId==null)
        //    {
        //        //Viewing own profile
        //        profileId = GetProfileId();
        //        ownProfile = true;
        //    }
            
        //    // Get profile info
        //    var profile = _profileRepo.GetProfile((int)profileId);
        //    var profileText = _profileRepo.GetProfileText((int)profileId);
        //    var profileDetail = _profileRepo.GetProfileDetail((int)profileId);

        //    // Calculate match between users
        //    var match = await _webClient.CalculateMatchAsync((int)profileId);

        //    // Get the profile details as a dictionary
        //    var detailDict = new Dictionary<string, string>();
        //    foreach (var prop in profileDetail.GetType().GetProperties())
        //    {
        //        var val = _profileRepo.GetOptionValue(prop.Name, (byte)prop.GetValue(profileDetail));
        //        detailDict.Add(prop.Name, val);
        //    }
            
        //    // Populate the view model
            


        //    var vm = new ProfileViewModel(profile);

        //    vm.ProfileText = profile.ProfileText;

        //    //It's possible they don't have any profile text..return empty object
        //    if (vm.ProfileText == null)
        //    {
        //        vm.ProfileText = new ProfileText();
        //    }

        //    return View(vm);
        //}

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