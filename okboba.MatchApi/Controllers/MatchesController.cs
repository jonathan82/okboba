using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using okboba.Entities;
using okboba.MatchCalculator;
using okboba.Repository;
using okboba.Repository.EntityRepository;
using okboba.Repository.MemoryRepository;
using okboba.Repository.Models;
using okboba.Repository.RedisRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
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
    [System.Web.Mvc.Authorize]
    public class MatchesController : OkbBaseController
    {
        private IMatchRepository _matchRepo;
        private RedisMatchRepository _redisRepo;
        private MatchCalc _matchCalc;
        const int NUM_MATCHES_PER_PAGE = 28;

        public MatchesController()
        {
            _matchRepo = MemoryMatchRepository.Instance;
            _redisRepo = RedisMatchRepository.Instance;
            _matchCalc = MatchCalc.Instance;
        }

        /// <summary>
        /// Calculates match between myself and another user
        /// </summary>    
        [System.Web.Http.HttpGet]    
        public MatchModel CalculateMatch(int otherProfileId)
        {
            var myProfileId = GetProfileId();
            var result = _matchRepo.CalculateMatch(myProfileId, otherProfileId);
            return result;
        }

        /// <summary>
        /// Gets the matches for the logged in user for a given page.  Looks in cache first and if not there
        /// calculate the matches and store in cache.  Returns a JSON array.
        /// </summary>        
        public HttpResponseMessage GetMatches([FromUri]MatchCriteriaModel criteria, int page = 1)
        {
            //We should be authenticated at this point.  Only allow users to get their own matches
            var profileId = GetProfileId();

            var key = _redisRepo.FormatKey(profileId, criteria);
            string json;

            //Check if in cache, if so use it.  otherwise calculate matches and store in cache
            if (_redisRepo.HasMatches(key))
            {
                //Get cached results
                int count = _redisRepo.GetMatchCount(key);
                var range = PageRange(page, count);

                if (range==null)
                {
                    json = "[]";
                }
                else
                {
                    var jsonList = _redisRepo.GetMatches(key, range.Item1, range.Item2);
                    json = "[" + string.Join(",", jsonList) + "]";
                }                
            }
            else
            {
                //calculate matches and save in cache
                var matches = _matchRepo.MatchSearch(profileId, criteria);
                var range = PageRange(page, matches.Count);

                _redisRepo.SaveMatchResults(key, matches);

                if (range==null)
                {
                    json = "[]";
                }
                else
                {
                    json = "[";
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        //serialize matches[i] to return to caller                    
                        json += JsonConvert.SerializeObject(matches[i]) + ",";
                    }
                    json = json.TrimEnd(',');
                    json += "]";
                }                
            }

            //var response = Request.CreateResponse(HttpStatusCode.OK, json, "application/json");
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }  

        private string GetCachedResults(int page, string key)
        {
            var json = "";
            //Get cached results
            int count = _redisRepo.GetMatchCount(key);
            var range = PageRange(page, count);

            if (range == null)
            {
                json = "[]";
            }
            else
            {
                var jsonList = _redisRepo.GetMatches(key, range.Item1, range.Item2);
                json = "[" + string.Join(",", jsonList) + "]";
            }
            return json;
        }

        /// <summary>
        /// Calculates the start and end indexes for a given page.  Checks that page is within range.
        /// Returns null if page outside of range.
        /// </summary>
        private Tuple<int, int> PageRange(int page, int count)
        {
            // Sanity check for page
            if (page < 1 || page > Math.Ceiling((float)count / NUM_MATCHES_PER_PAGE))
            {
                //return null to indicate page is outside of valid range
                //page = 1;
                return null;
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
