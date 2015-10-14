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
        public string Truncate(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars);
        }

        protected int GetProfileId()
        {            
            int profileId;

            //Check if ProfileId in session, if not cache it there
            if (Session["ProfileId"] == null)
            {
                var db = new OkbDbContext();
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                profileId = user.Profile.Id;
                Session["ProfileId"] = profileId;
            }
            else
            {
                profileId = (int)Session["ProfileId"];
            }

            return profileId;
        }

        protected Profile GetUserProfile()
        {
            //Check if user is loggged in first
            if(!User.Identity.IsAuthenticated)
            {
                return null;
            }

            var profileId = GetProfileId();

            //Get the profile
            var db = new OkbDbContext();
            var profile = db.Profiles.Find(profileId);

            return profile;
        }
    }
}