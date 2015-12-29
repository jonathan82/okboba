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
        private IActivityRepository _feedRepo;
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
            _feedRepo = EntityActivityRepository.Instance;
        }

        [ChildActionOnly]
        public ActionResult ProfileDetailAndText(int profileId, bool isMe)
        {
            // Get the profile text
            var profileText = _profileRepo.GetProfileText(profileId);

            // Get the profile details as a dictionary
            var profileDetail = _profileRepo.GetProfileDetail(profileId);
            var detailOptions = _profileRepo.GetDetailOptions();
            //var detailDict = new Dictionary<string, string>();
            //foreach (var prop in profileDetail.GetType().GetProperties())
            //{
            //    if (prop.PropertyType != typeof(byte)) continue;
            //    var val = _profileRepo.GetOptionValue(prop.Name, (byte)(prop.GetValue(profileDetail)));
            //    detailDict.Add(prop.Name, val);
            //}

            //Populate view model
            var vm = new ProfileDetailViewModel
            {
                ProfileText = profileText,
                ProfileDetail = profileDetail,
                //DetailDict = detailDict,                
                DetailOptions = detailOptions,
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

        /// <summary>
        /// 
        /// Action for the default Profile view.  Takes a string of the user's Base 62 ID and gets the Profile Id
        /// to pass to the child views. Should eventually support anonymous viewing of profiles based on a privacy
        /// flag.
        /// 
        /// </summary>
        public ActionResult Index(string userId)
        {
            var me = GetProfileId();
            var vm = new ProfileViewModel();            

            if (string.IsNullOrEmpty(userId) || userId == User.Identity.GetUserId())
            {
                //Viewing own profile
                vm.ProfileId = me;
                vm.IsMe = true;
            }
            else
            {
                //get the profile id associated with the passed in userId
                var id = _profileRepo.GetProfileId(userId);

                if (id < 0 )
                {
                    // Bad userId passed in - no profile found!
                    // throw exception or return no profile found view??
                    throw new Exception("No Profile Found for given user Id!");
                }

                vm.ProfileId = id;
                vm.IsMe = false;
            }
           
            return View(vm);
        }
        
        /// <summary>
        /// API: edit the profile details
        /// </summary>
        [HttpPost]
        public ActionResult EditDetail(ProfileDetail details, int section)
        {
            var me = GetProfileId();

            _profileRepo.EditDetails(details, (OkbConstants.ProfileDetailSections)section, me);

            return Redirect("/profile");
        }

        /// <summary>
        /// API: Update profile text
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditProfileText(string text, string whichQuestion)
        {
            //Massage the input
            text = Truncate(text, OkbConstants.MAX_PROFILE_TEXT_SIZE);
            text = HttpContext.Server.HtmlEncode(text);

            _profileRepo.EditProfileText(GetProfileId(), text, whichQuestion);
            _feedRepo.EditProfileTextActivity(GetProfileId(), text);

            return Content("{}");
        }
    }
}