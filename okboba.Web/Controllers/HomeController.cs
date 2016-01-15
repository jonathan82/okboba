﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using okboba.Resources;
using okboba.Repository;
using Newtonsoft.Json;
using okboba.Repository.EntityRepository;
using okboba.Repository.RedisRepository;

namespace okboba.Web.Controllers
{
    
    [Authorize]
    public class HomeController : OkbBaseController
    {
        private IActivityRepository _activityRepo;
        private IProfileRepository _profileRepo;
        private IRedisMatchRepository _matchCache;
        private IQuestionRepository _quesRepo;

        public HomeController()
        {
            _activityRepo = EntityActivityRepository.Instance;
            _profileRepo = EntityProfileRepository.Instance;
            _matchCache = SXMatchRepository.Instance;
            _quesRepo = EntityQuestionRepository.Instance;
        }

        [ChildActionOnly]
        public ActionResult RecommendedMatches()
        {            
            var me = GetProfileId();

            var criteria = _profileRepo.GetMatchCriteria(me);

            var matches = _matchCache.Recommended(me, criteria, OkbConstants.MATCHES_RECOMMENDED_RETURN, OkbConstants.MATCHES_RECOMMENDED_CONSIDERED);

            if (matches==null)
            {
                //cache miss
                var webClient = GetMatchApiClient();
                webClient.CalculateAndSaveMatchesAsync(criteria).Wait();
            }

            //get matches 2nd try
            matches = _matchCache.Recommended(me, criteria, OkbConstants.MATCHES_RECOMMENDED_RETURN, OkbConstants.MATCHES_RECOMMENDED_CONSIDERED);

            return PartialView("_MatchCarousel", matches);
        }

        [ChildActionOnly]
        public ActionResult ActivityFeed()
        {                    
            //Get all activities for now
            var vm = _activityRepo.GetActivities(OkbConstants.NUM_ACTIVITIES_TO_SHOW);

            return PartialView("_ActivityFeed",vm);
        }

        [ChildActionOnly]
        public ActionResult ActiveUsers()
        {
            var activeUsers = _activityRepo.GetActiveUsers();

            return PartialView("_ActiveUsers", activeUsers);
        }

        public ActionResult Index()
        {
            var me = GetProfileId();

            var ques = _quesRepo.Next2Questions(me);

            return View(ques);
        }

    }
}