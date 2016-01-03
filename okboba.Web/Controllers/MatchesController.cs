using okboba.Repository;
using okboba.Repository.EntityRepository;
using okboba.Repository.Models;
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

namespace okboba.Controllers
{
    [Authorize]
    public class MatchesController : OkbBaseController
    {
        private MatchApiClient _webClient;
        private IProfileRepository _profileRepo;

        public MatchesController()
        {
            _profileRepo = EntityProfileRepository.Instance;
        }

        // GET: Matches
        public async Task<ActionResult> Index(MatchCriteriaModel criteria)
        {
            _webClient = GetMatchApiClient();

            var profileId = GetProfileId();
            var profile = _profileRepo.GetProfile(profileId);
            
            //For now, if criteria isn't specified lets choose sensible defaults based on user                        
            if(criteria.Gender==OkbConstants.UNKNOWN_GENDER)
            {
                criteria.Gender = profile.Gender == OkbConstants.FEMALE ? OkbConstants.MALE : OkbConstants.FEMALE;
            }

            //Call the MatchApi client to get the first page of matches
            var matches = await _webClient.GetMatchesAsync(criteria);

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