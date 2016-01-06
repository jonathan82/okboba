using okboba.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository
{
    public interface IMatchRepository
    {
        List<MatchModel> Search(int profileId, MatchCriteriaModel criteria);
        MatchModel Calculate(int profileId1, int profileId2);
    }
}
