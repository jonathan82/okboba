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
using okboba.Web.Models;
using okboba.Repository;
using System.Threading;
using okboba.Web.Helpers;
using okboba.Resources.Exceptions;
using System.Security.Claims;

namespace okboba.Web.Controllers
{
    public class OkbBaseController : Controller
    {
        private IProfileRepository _profileRepo;
        private IMessageRepository _msgRepo;

        public OkbBaseController()
        {
            _profileRepo = EntityProfileRepository.Instance;
            _msgRepo = EntityMessageRepository.Instance;
        }

        public string Truncate(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars);
        }

        /// <summary>
        /// Gets the profile Id of the logged in user. Profile Id is stored as a Claim in the user's cookie.
        /// </summary>
        protected int GetMyProfileId()
        {            
            //Get profile Id from claims
            var identity = User.Identity as ClaimsIdentity;

            foreach (var claim in identity.Claims)
            {
                if (claim.Type == OkbConstants.PROFILEID_CLAIM)
                {
                    return Convert.ToInt32(claim.Value);
                }
            }

            //Error if we got here - no claim found
            throw new Exception("No profile Id found for logged in user: " + User.Identity.Name);            
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

        /// <summary>
        /// Sets the users language by looking at the cookie
        /// </summary>
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = null;

            // Attempt to read the culture cookie from Request
            HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null)
                cultureName = cultureCookie.Value;
            else
                cultureName = CultureHelper.GetDefaultCulture(); //default culture is chinese
                //cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                //        Request.UserLanguages[0] :  // obtain it from HTTP header AcceptLanguages
                //        null;
            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            return base.BeginExecuteCore(callback, state);
        }

        [ChildActionOnly]
        [Authorize]
        public ActionResult Navbar()
        {
            var me = GetMyProfileId();

            var matchClient = GetMatchApiClient();

            var vm = new NavbarViewModel();

            var profileText = _profileRepo.GetProfileText(me);

            vm.MyProfile = _profileRepo.GetProfile(me);
            vm.UnreadCount = _msgRepo.GetUnreadCount(me);
            vm.HasPhoto = vm.MyProfile.GetFirstHeadshot() != "";
            vm.HasProfileText = !string.IsNullOrEmpty(profileText.Question1);
            vm.NumQuesAnswered = matchClient.GetAnswerCountAsync(me).Result; 

            return PartialView("_Navbar", vm);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if(filterContext.Exception is UserNotFoundException)
            {
                //show error page
                filterContext.ExceptionHandled = true;
                filterContext.Result = View("UserNotFound");
            }

            //base.OnException(filterContext);    
        }
    }
}