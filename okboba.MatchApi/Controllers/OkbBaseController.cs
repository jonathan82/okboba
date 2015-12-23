using Microsoft.AspNet.Identity;
using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace okboba.MatchApi.Controllers
{
    [EnableCors(origins: "http://dev.okboba.com", headers: "*", methods: "*", SupportsCredentials = true)]
    public class OkbBaseController : ApiController
    {
        protected int GetProfileId()
        {
            int profileId;

            var db = new OkbDbContext();
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (user == null)
            {
                // No profile exists for user!??!?
                throw new Exception("No profile exists for user");
            }

            profileId = user.Profile.Id;

            return profileId;
        }
    }
}