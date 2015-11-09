﻿using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using okboba.Entities;
using okboba.MatchCalculator;
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
    [EnableCors(origins: "http://dev.okboba.com", headers: "*", methods: "*", SupportsCredentials = true)]
    public class MatchesController : Controller
    {
        private IMatchRepository _matchRepo;
        private RedisMatchRepository _redisRepo;
        private MatchCalc _matchCalc;
        const int NUM_MATCHES_PER_PAGE = 28;

        public MatchesController()
        {
            _matchRepo = EntityMatchRepository.Instance;
            _redisRepo = RedisMatchRepository.Instance;
            _matchCalc = MatchCalc.Instance;
        }

        /// <summary>
        /// Updates the users cached answer used for matching.  Only allow users to update their own answers.
        /// </summary>
        public JsonResult UpdateCacheAnswer(Answer answer)
        {
            //We should be authenticated at this point.  Only allow users to update their own answers
            var profileId = GetProfileId();

            answer.ProfileId = profileId;

            _matchCalc.AddOrUpdate(answer);

            return Json(new { result = "success" });
        }

        /// <summary>
        /// Gets the matches for the logged in user for a given page.  Looks in cache first and if not there
        /// calculate the matches and store in cache.  Returns a JSON array.
        /// </summary>        
        public ActionResult GetMatches(MatchCriteriaModel criteria, int page = 1)
        {
            //We should be authenticated at this point.  Only allow users to get their own matches
            var profileId = GetProfileId();

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

                _redisRepo.SaveMatchResults(key, matches);

                json = "[";
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    //serialize matches[i] to return to caller                    
                    json += JsonConvert.SerializeObject(matches[i]) + ",";                    
                }
                json = json.TrimEnd(',');
                json += "]";
            }

            return Content(json, "application/json");
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

        protected int GetProfileId()
        {
            int profileId;

            //Check if ProfileId in session, if not cache it there
            if (Session["ProfileId"] == null)
            {
                var db = new OkbDbContext();
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);

                if (user == null)
                {
                    // No profile exists for user!??!?
                    throw new Exception("No profile exists for user");
                }

                profileId = user.Profile.Id;
                Session["ProfileId"] = profileId;
            }
            else
            {
                profileId = (int)Session["ProfileId"];
            }

            return profileId;
        }
    }
}
