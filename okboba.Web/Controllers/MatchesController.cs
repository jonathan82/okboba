using okboba.Repository;
using okboba.Repository.EntityRepository;
using okboba.Repository.Models;
using okboba.Repository.WebClient;
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


        // GET: Matches
        public async Task<ActionResult> Index(MatchCriteriaModel criteria)
        {
            InitClient();

            var profileId = GetProfileId();
            var profile = _profileRepo.GetProfile(profileId);
            
            //For now, if criteria isn't specified lets choose sensible defaults based on user                        
            if(criteria.Gender==OkbConstants.UNKNOWN_GENDER)
            {
                //criteria.Gender = profile.Gender == "女" ? "男" : "女";
                criteria.Gender = profile.Gender == OkbConstants.FEMALE ? OkbConstants.MALE : OkbConstants.FEMALE;
            }

            //Call the MatchApi client to get the first page of matches
            var matches = await _webClient.GetMatchesAsync(criteria);

            var vm = new MatchesViewModel
            {
                Matches = matches,
                StorageUrl = ConfigurationManager.AppSettings["StorageUrl"],
                MatchApiUrl = ConfigurationManager.AppSettings["MatchApiUrl"],
                MatchCriteria = criteria
            };

            return View(vm);
        }
    }
}