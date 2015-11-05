using okboba.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace okboba.Controllers
{
    [Authorize]
    public class MatchesController : OkbBaseController
    {
        // GET: Matches
        public ActionResult Index(MatchCriteriaModel criteria)
        {
            //Get the logged in users matches and returns a view with the first page of results
            //Takes a MatchSearchCriteria object to search for the matches
            //  Saves the users search criteria
            //If MatchSearchCriteria is null then get the users saved search criteria from the database
            //Criteria can be:
            //  Gender, location 1

            var profileId = GetProfileId();

            if(criteria==null)
            {
                //Get the criteria from the database
            }

            //Call the MatchApi client to get the first page of matches

            return View();
        }
    }
}