using Microsoft.AspNet.Identity;
using okboba.Entities;
using okboba.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Cors;

namespace okboba.MatchApi.Controllers
{
    [EnableCors(origins: "http://dev.okboba.com", headers: "*", methods: "*", SupportsCredentials = true)]
    public class OkbBaseController : ApiController
    {
        //protected int GetProfileId()
        //{
        //    int profileId;

        //    var db = new OkbDbContext();
        //    var userId = User.Identity.GetUserId();
        //    var user = db.Users.Find(userId);

        //    if (user == null)
        //    {
        //        // No profile exists for user!??!?
        //        throw new Exception("No profile exists for user");
        //    }

        //    profileId = user.Profile.Id;

        //    return profileId;
        //}

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
    }
}