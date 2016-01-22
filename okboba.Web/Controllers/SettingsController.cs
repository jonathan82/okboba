using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using okboba.Repository;
using okboba.Repository.EntityRepository;
using okboba.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace okboba.Web.Controllers
{
    [Authorize]
    public class SettingsController : OkbBaseController
    {
        private ILocationRepository _locRepo;
        private IProfileRepository _profileRepo;

        public ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        public SettingsController()
        {
            _locRepo = EntityLocationRepository.Instance;
            _profileRepo = EntityProfileRepository.Instance;
        }

        // GET: Settings
        public ActionResult Index()
        {
            var vm = new SettingsViewModel();

            var me = GetProfileId();

            var user = UserManager.FindByName(User.Identity.Name);
            var profile = _profileRepo.GetProfile(me);

            vm.Email = user.Email;
            vm.LocationString = _locRepo.GetLocationString(profile.LocationId1, profile.LocationId2);
            vm.LocationId1 = profile.LocationId1;
            vm.LocationId2 = profile.LocationId2;
            vm.Provinces = _locRepo.GetProvinces();

            return View(vm);
        }

        // GET
        public ActionResult ChangePassword()
        {
            return View();
        }
    }
}