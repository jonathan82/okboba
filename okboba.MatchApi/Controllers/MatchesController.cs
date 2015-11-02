using okboba.Repository;
using okboba.Repository.EntityRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace okboba.MatchApi.Controllers
{
    public class MatchesController : ApiController
    {
        private IMatchRepository _matchRepo;

        public MatchesController()
        {
            _matchRepo = EntityMatchRepository.Instance;
        }
    }
}
