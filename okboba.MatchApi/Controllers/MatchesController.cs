using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using okboba.Entities;
using okboba.MatchCalculator;
using okboba.Repository;
using okboba.Repository.EntityRepository;
using okboba.Repository.MemoryRepository;
using okboba.Repository.Models;
using okboba.Repository.RedisRepository;
using okboba.Resources;
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
        private IRedisMatchRepository _redisRepo;

        public MatchesController()
        {
            _matchRepo = EntityMatchRepository.Instance;
            _redisRepo = SXRedisMatchRepository.Instance;
        }

        /// <summary>
        /// Calculates match between myself and another user
        /// </summary>    
        [System.Web.Http.HttpGet]    
        public MatchModel CalculateMatch(int otherProfileId)
        {
            var myProfileId = GetProfileId();
            var result = _matchRepo.Calculate(myProfileId, otherProfileId);
            return result;
        }

        /// <summary>
        /// Gets the matches for the logged in user for a given page.  Looks in cache first and if not there
        /// calculate the matches and store in cache.  Returns a JSON array.
        /// </summary>        
        public HttpResponseMessage GetMatches([FromUri]MatchCriteriaModel criteria, int page = 1)
        {
            //We should be authenticated at this point.  Only allow users to get their own matches
            var me = GetProfileId();

            string json = "";

            var matches = _redisRepo.Get(me, criteria, page);

            if (matches != null)
            {
                //Hit - results in cache.  Serialize them to JSON
                json = JsonConvert.SerializeObject(matches);                               
            }
            else
            {
                //Miss - calculate matches and save in cache. 
                matches = _matchRepo.Search(me, criteria);

                _redisRepo.Save(me, matches, criteria);

                //Serialize them to JSON
                int start = (page - 1) * OkbConstants.MATCHES_PER_PAGE;
                var pagedMatches = matches.Skip(start).Take(OkbConstants.MATCHES_PER_PAGE);

                json = JsonConvert.SerializeObject(pagedMatches);
            }

            //var response = Request.CreateResponse(HttpStatusCode.OK, json, "application/json");
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }  


        //////////////////////// Private Methods ////////////////////////////////////////
        //private string SerializeMatches(IEnumerable<MatchModel> matches)
        //{
        //    var json = "[";
        //    foreach (var match in matches)
        //    {
        //        json += JsonConvert.SerializeObject(match);
        //    }
        //}
        
        /// <summary>
        /// Calculates the start and end indexes for a given page.  Checks that page is within range.
        /// Returns null if page outside of range.
        /// </summary>
        //private Tuple<int, int> PageRange(int page, int count)
        //{
        //    // Sanity check for page
        //    if (page < 1 || page > Math.Ceiling((float)count / NUM_MATCHES_PER_PAGE))
        //    {
        //        //return null to indicate page is outside of valid range
        //        //page = 1;
        //        return null;
        //    }

        //    int start, end;
        //    start = (page - 1) * NUM_MATCHES_PER_PAGE;
        //    end = start + NUM_MATCHES_PER_PAGE;

        //    if (end > count)
        //    {
        //        end = count;
        //    }

        //    return new Tuple<int, int>(start, end);
        //}
        
    }
}
