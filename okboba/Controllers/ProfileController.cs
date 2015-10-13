using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using okboba.Web.Models;

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

            //Return the view model
            return View(vm);
        }
    }
}