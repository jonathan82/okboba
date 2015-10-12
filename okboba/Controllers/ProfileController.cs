using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace okboba.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        // Show my own Profile
        public ActionResult Index()
        {
            //Get the currently logged in user
            //User.Identity.

            //Get the user profile from DB
            OkbDbContext db = new OkbDbContext();            

            //Return the view model
            return View();
        }
    }
}