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
    public class ProfileController : Controller
    {
        // GET: Profile
        // Show my own Profile
        public ActionResult Index()
        {
            //Get the currently logged in user
            var profileId = Session["ProfileId"];

            //Get the user profile from DB
            OkbDbContext db = new OkbDbContext();
            var profile = db.Profiles.Find(profileId);

            var vm = new ProfileViewModel { Profile = profile };

            //Return the view model
            return View(vm);
        }
    }
}