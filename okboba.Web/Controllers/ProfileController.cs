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
using okboba.Resources;

namespace okboba.Controllers
{

    [Authorize]
    public class ProfileController : OkbBaseController
    {
        private IProfileRepository _profileRepo;
        private ILocationRepository _locationRepo;
        private IActivityRepository _activityRepo;
        private MatchApiClient _webClient;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //base.OnActionExecuting(filterContext);
            _webClient = GetMatchApiClient();
        }

        public ProfileController()
        {
            _profileRepo = EntityProfileRepository.Instance;
            _locationRepo = EntityLocationRepository.Instance;
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
                Location = _locationRepo.GetLocationString(profile.LocationId1, profile.LocationId2),
                Section = section
            };

            return PartialView("_ProfileHeader", vm);
        }

        public ActionResult Index(int? id)
        {
            var me = GetProfileId();
            var vm = new ProfileViewModel();

            if (id == null || id == me)
            {
                //Viewing own profile
                vm.ProfileId = me;
                vm.IsMe = true;
            }
            else
            {
                vm.ProfileId = (int)id;
                vm.IsMe = false;
            }
           
            return View(vm);
        }
        

        /// <summary>
        /// API: Update profile text
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditProfileText(string qText, string whichQuestion)
        {
            //Massage the input
            qText = Truncate(qText, OkbConstants.MAX_PROFILE_TEXT_SIZE);
            qText = HttpContext.Server.HtmlEncode(qText);

            _profileRepo.EditProfileText(GetProfileId(), qText, whichQuestion);
            _activityRepo.EditProfileTextActivity(GetProfileId(), Truncate(qText, 100));

            return Content("{}");
        }
    }
}