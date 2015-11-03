using Newtonsoft.Json;
using okboba.Repository;
using okboba.Repository.EntityRepository;
using okboba.Repository.Models;
using okboba.Repository.RedisRepository;
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
        private RedisMatchRepository _redisRepo;
        const int NUM_MATCHES_PER_PAGE = 25;

        public MatchesController()
        {
            _matchRepo = EntityMatchRepository.Instance;
            _redisRepo = RedisMatchRepository.Instance;
        }

        /// <summary>
        /// Gets the matches for a given user for a given page.  Looks in cache first and if not there
        /// calculate the matches and store in cache.  Returns a JSON array.
        /// </summary>        
        public JsonResult GetMatches(int profileId, int page, MatchCriteriaModel criteria)
        {
            var key = _redisRepo.FormatKey(profileId, criteria);
            string json;

            //Check if in cache, if so use it.  otherwise calculate matches and store in cache
            if(_redisRepo.HasMatches(key))
            {
                //Get cached results
                int count = _redisRepo.GetMatchCount(key);
                var range = PageRange(page, count);
                var jsonList = _redisRepo.GetMatches(key, range.Item1, range.Item2);
                json = "[" + string.Join(",", jsonList) + "]";              
            }
            else
            {
                //calculate matches and save in cache
                var matches = _matchRepo.MatchSearch(profileId, criteria);
                var range = PageRange(page, matches.Count);

                _redisRepo.SaveMatchResults(profileId, criteria, matches);

                json = "[";
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    //serialize matches[i] to return to caller                    
                    json += JsonConvert.SerializeObject(matches[i]);                    
                }
                json += "]";
            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Calculates the start and end indexes for a given page.  Checks that page is within range.
        /// </summary>
        private Tuple<int, int> PageRange(int page, int count)
        {
            // Sanity check for page
            if (page < 1 || page > Math.Ceiling((float)count / NUM_MATCHES_PER_PAGE))
            {
                page = 1;
            }

            int start, end;
            start = (page - 1) * NUM_MATCHES_PER_PAGE;
            end = start + NUM_MATCHES_PER_PAGE;

            if (end > count)
            {
                end = count;
            }

            return new Tuple<int, int>(start, end);
        }
    }
}
