﻿using Newtonsoft.Json;
using okboba.Repository;
using okboba.Repository.EntityRepository;
using okboba.Repository.Models;
using okboba.Repository.RedisRepository;
using okboba.Repository.WebClient;
using okboba.Resources;
using okboba.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace okboba.Web.Controllers
{
    [Authorize]
    public class MatchesController : OkbBaseController
    {
        private MatchApiClient _webClient;
        private IProfileRepository _profileRepo;
        private IRedisMatchRepository _matchCache;

        public MatchesController()
        {
            _profileRepo = EntityProfileRepository.Instance;
            _matchCache = SXMatchRepository.Instance;
        }

        /// <summary>
        /// Gets the matches for a give page.  Looks in the cache first and if not there calculates
        /// matches and saves them there. If the forceRecalculation flag is true skip looking in the cache.
        /// </summary>
        private async Task<IList<MatchModel>> GetMatchesAsync(int profileId, MatchCriteriaModel criteria, int page, bool forceRecalc = false)
        {
            //check if matches in cache
            var matches = _matchCache.Get(profileId, criteria, page);

            if (matches == null || forceRecalc)
            {
                //cache miss
                await _webClient.CalculateAndSaveMatchesAsync(criteria);
                Session[OkbConstants.FORCE_RECALCULATE_MATCHES] = false;
            }

            //get matches from cache - 2nd try
            matches = _matchCache.Get(profileId, criteria, page);

            return matches;
        }

        /// <summary>
        /// API: returns a list of matches for the given page and criteria. Called by
        /// AJAX and returns JSON. Looks in the cache first and if not there call
        /// the match server to calculate matches and save in cache.
        /// </summary>
        public async Task<ActionResult> Get(MatchCriteriaModel criteria, int page = 1)
        {
            var me = GetMyProfileId();

            var matches = await GetMatchesAsync(me, criteria, page);

            //Serialize matches to JSON
            var json = JsonConvert.SerializeObject(matches);

            return Content(json, "application/json");
        }


        // GET: Matches
        public async Task<ActionResult> Index()
        {
            _webClient = GetMatchApiClient(); //need to initialize here since we're using cookie authentication

            var me = GetMyProfileId();

            //Get user's search criteria
            var criteria = _profileRepo.GetMatchCriteria(me);

            //check for forceRecalculation flag
            bool forceRecalc = false;
            if (Session[OkbConstants.FORCE_RECALCULATE_MATCHES] != null && 
                (bool)Session[OkbConstants.FORCE_RECALCULATE_MATCHES] == true)
            {
                forceRecalc = true;
            }            

            var matches = await GetMatchesAsync(me, criteria, 1, forceRecalc); //first page

            var vm = new MatchesViewModel
            {
                Matches = matches,
                MatchApiUrl = ConfigurationManager.AppSettings["MatchApiUrl"],
                StorageUrl = ConfigurationManager.AppSettings["StorageUrl"] + OkbConstants.PHOTO_CONTAINER + "/",
                MatchCriteria = criteria
            };

            return View(vm);
        }
    }
}