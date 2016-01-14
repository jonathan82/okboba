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
using okboba.Resources;
using okboba.Repository.RedisRepository;
using okboba.Repository.EntityRepository;

namespace okboba.Web.Controllers
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


        /// <summary>
        /// Returns true if it is ok to add the activity to the feed. It is OK if the time
        /// elapsed is less than the pre-defined interval since the last time the activity was added.
        /// We use Session to store the last added activity time. Prevents the activity feed from
        /// being overloaded with a user's activities.
        /// </summary>
        protected bool IsOkToAddActivity(OkbConstants.ActivityCategories category)
        {            
            DateTime threshold;

            switch (category)
            {
                case OkbConstants.ActivityCategories.Joined:
                    return true;
                case OkbConstants.ActivityCategories.UploadedPhoto:
                    threshold = DateTime.Now.AddMinutes(-OkbConstants.ACTIVITY_UPLOADEDPHOTO_INTERVAL);
                    break;
                case OkbConstants.ActivityCategories.EditedProfileText:
                    threshold = DateTime.Now.AddMinutes(-OkbConstants.ACTIVITY_EDITEDPROFILE_INTERVAL);
                    break;
                case OkbConstants.ActivityCategories.AnsweredQuestion:
                    threshold = DateTime.Now.AddMinutes(-OkbConstants.ACTIVITY_ANSWEREDQUESTION_INTERVAL);
                    break;
                default:
                    //throw exception?
                    return true;
            }

            var lastAdded = Session["Activity:" + category.ToString()];

            if (lastAdded == null || (DateTime)lastAdded < threshold)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates the activity's last added date in the Session
        /// </summary>
        protected void UpdateActivityLastAdded(OkbConstants.ActivityCategories category)
        {
            if (category == OkbConstants.ActivityCategories.Joined) return; //no need to store join activity
            Session["Activity:" + category.ToString()] = DateTime.Now;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!Request.IsAuthenticated)
            {
                return;
            }

            //Get unread message count 
            var msgRepo = EntityMessageRepository.Instance;
            var me = GetProfileId();
            ViewBag.UnreadCount = msgRepo.GetUnreadCount(me);
            base.OnActionExecuted(filterContext);
        }
    }
}