using okboba.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository
{
    public interface IRedisMatchRepository
    {
        void Save(int profileId, IList<MatchModel> matches, MatchCriteriaModel criteria);
        IList<MatchModel> Get(int profileId, MatchCriteriaModel criteria, int page = 1);
        IList<MatchModel> Recommended(int profileId, MatchCriteriaModel criteria, int numReturn, int numConsidered);
    }
}
