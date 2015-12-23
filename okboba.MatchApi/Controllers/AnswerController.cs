using okboba.Entities;
using okboba.MatchCalculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace okboba.MatchApi.Controllers
{
    [Authorize]
    public class AnswerController : OkbBaseController
    {
        //// Private Variables
        private MatchCalc _matchCalc;

        //// Constructor
        public AnswerController()
        {
            _matchCalc = MatchCalc.Instance;
        }

        // POST api/<controller>
        /// <summary>
        /// Updates/Adds the users answer in the answer cache.
        /// </summary>
        public void Post([FromBody]Answer answer)
        {
            //We should be authenticated at this point.  Only allow users to update their own answers
            var profileId = GetProfileId();

            answer.ProfileId = profileId;

            //we don't do any validation since all of it is done upstream before we get here.
            _matchCalc.AddOrUpdate(answer);
        }
    }
}