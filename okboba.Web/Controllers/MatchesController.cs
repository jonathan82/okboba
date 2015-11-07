using okboba.Repository.Models;
using okboba.Repository.WebClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace okboba.Controllers
{
    [Authorize]
    public class MatchesController : OkbBaseController
    {
        private MatchApiClient _webClient;

        private void InitClient()
        {
            //get the cookie and base
            var cookie = new Cookie();

            cookie.Name = Startup.IDENTITY_COOKIE_NAME;
            //cookie.Domain = HttpContext.Request.Cookies[Startup.IDENTITY_COOKIE_NAME].Domain;
            cookie.Value = HttpContext.Request.Cookies[Startup.IDENTITY_COOKIE_NAME].Value;

            var url = ConfigurationManager.AppSettings["MatchApiUrl"];
            _webClient = new MatchApiClient(url, cookie);
        }

        public MatchesController()
        {
            
        }

        // GET: Matches
        public async Task<ActionResult> Index(MatchCriteriaModel criteria)
        {
            //Get the logged in users matches and returns a view with the first page of results
            //Takes a MatchSearchCriteria object to search for the matches
            //  Saves the users search criteria
            //If MatchSearchCriteria is null then get the users saved search criteria from the database
            //Criteria can be:
            //  Gender, location 1

            InitClient();

            var profileId = GetProfileId();

            if (criteria == null)
            {
                //Get the criteria from the database
                //Use default options
                criteria = new MatchCriteriaModel()
                {
                    Gender = "M",
                    LocationId1 = 1
                };
            }

            //Call the MatchApi client to get the first page of matches
            var matches = await _webClient.GetMatchesAsync(criteria);

            return View(matches);
        }
    }
}