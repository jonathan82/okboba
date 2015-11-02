using okboba.Repository;
using okboba.Repository.EntityRepository;
using okboba.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

/// <summary>
/// MatchApi contains API controllers to get matches for a user.  It will return only what's necessary
/// (ie for a given page).  It will first look in the cache, and if not there call MatchCalc to get the 
/// matches, put them in the cache, and return them to the caller.
/// 
/// Possible Flow 1: user ===> okboba.Web ===> okboba.MatchApi
/// 
/// Possible Flow 2: user ===> okboba.MatchApi (user calls MatchApi directly using jquery)
/// 
/// </summary>


namespace okboba.MatchApi.Controllers
{        
    public class MatchesController : Controller
    {
        private IMatchRepository _matchRepo;
        const int NUM_MATCHES_PER_PAGE = 25;

        public MatchesController()
        {
            _matchRepo = EntityMatchRepository.Instance;
        }

        public JsonResult GetMatches(int profileId, int page, MatchCriteriaModel criteria)
        {
            var matches = _matchRepo.MatchSearch(profileId, criteria);

            // Sanity check for page
            if(page < 1 || page > Math.Ceiling((float)matches.Count / NUM_MATCHES_PER_PAGE))
            {
                page = 1;
            }

            int start, end;
            start = (page - 1) * NUM_MATCHES_PER_PAGE;
            end = start + NUM_MATCHES_PER_PAGE;

            if (end > matches.Count)
            {
                end = matches.Count;
            }

            var pagedMatches = new List<MatchModel>();

            for(int i= start; i < end; i++)
            {
                pagedMatches.Add(matches[i]);
            }

            return Json(pagedMatches, JsonRequestBehavior.AllowGet);
        }
    }
}
