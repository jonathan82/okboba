using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace okboba.Controllers
{
    public class OkbBaseController : Controller
    {
        protected Profile GetUserProfile()
        {
            OkbDbContext db;

            //Check if user is loggged in first
            if(!User.Identity.IsAuthenticated)
            {
                return null;
            }

            //Check if ProfileId in session, if not cache it there
            var profileId = Session["ProfileId"];
            if(profileId==null)
            {
                db = new OkbDbContext();
                var userId = User.Identity.GetUserId();                
                var user = db.Users.Find(userId);
                profileId = user.Profile.Id;
                Session["ProfileId"] = profileId;
            }

            //Get the profile
            db = new OkbDbContext();
            var profile = db.Profiles.Find(profileId);

            return profile;
        }
    }
}