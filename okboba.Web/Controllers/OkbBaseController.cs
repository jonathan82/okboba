using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using okboba.Repository.WebClient;
using System.Configuration;
using System.Net;

namespace okboba.Controllers
{
    public class OkbBaseController : Controller
    {
        public string Truncate(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars);
        }

        /// <summary>
        /// Gets the profile Id of the logged in user. First looks in the Session and if not
        /// there looks up in DB and caches in the session to speed up lookups next time.
        /// The User Id is stored in the Identity object of the current thread.
        /// </summary>
        protected int GetProfileId()
        {            
            int profileId;

            //Check if ProfileId in session, if not cache it there
            if (Session["ProfileId"] == null)
            {
                var db = new OkbDbContext();
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);

                if (user == null)
                {
                    // No profile exists for user!??!?
                    throw new Exception("No profile exists for user");
                }

                profileId = user.Profile.Id;
                Session["ProfileId"] = profileId;
            }
            else
            {
                profileId = (int)Session["ProfileId"];
            }

            return profileId;
        }

        /// <summary>
        /// Gets an instance of the MatchApiClient by creating it with an authentication cookie.
        /// This is created per request and should be called once the controller is constructed
        /// since we need the HttpContext object to ge the cookie info.
        /// </summary>
        protected MatchApiClient GetMatchApiClient()
        {
            //get the cookie and base
            var cookie = new Cookie();

            cookie.Name = Startup.IDENTITY_COOKIE_NAME;
            //cookie.Domain = HttpContext.Request.Cookies[Startup.IDENTITY_COOKIE_NAME].Domain;
            cookie.Value = HttpContext.Request.Cookies[Startup.IDENTITY_COOKIE_NAME].Value;

            var url = ConfigurationManager.AppSettings["MatchApiUrl"];

            return new MatchApiClient(url, cookie);
        }
    }
}