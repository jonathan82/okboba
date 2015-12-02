using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using okboba.Resources;
using okboba.Repository;
using Newtonsoft.Json;
using okboba.Repository.EntityRepository;

namespace okboba.Controllers
{
    
    [Authorize]
    public class HomeController : OkbBaseController
    {
        private IActivityRepository _activityRepo;
        private IProfileRepository _profileRepo;

        public HomeController()
        {
            _activityRepo = EntityActivityRepository.Instance;
            _profileRepo = EntityProfileRepository.Instance;
        }

        [ChildActionOnly]
        public ActionResult ActivityFeed()
        {
            //retrieve the preferences for the user and show 'em what they want
            var profile = _profileRepo.GetProfile(GetProfileId());

            var vm = _activityRepo.GetActivities(profile.LookingForGender, 50);

            return PartialView("_ActivityFeed",vm);
        }

        public ActionResult Index()
        {
            return View();
        }

    }
}