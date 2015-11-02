using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using okboba.Repository;
using okboba.Repository.Models;
using okboba.Entities;
using okboba.MatchCalculator;

namespace okboba.Repository.EntityRepository
{
    public class EntityMatchRepository : IMatchRepository
    {
        #region Singelton
        private static EntityMatchRepository instance;
        private EntityMatchRepository()
        {
            _matchCalc = MatchCalc.Instance;
        }

        public static EntityMatchRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EntityMatchRepository();
                }
                return instance;
            }
        }
        #endregion

        //////////////////// Member variables /////////////////
        private MatchCalc _matchCalc;


        /// <summary>
        /// Peforms a match search for a user given their search preferences and returns a list
        /// of sorted matches.
        /// </summary>
        List<MatchModel> IMatchRepository.MatchSearch(int profileId, string gender, int locId1)
        {
            var db = new OkbDbContext();
            var matches = new List<MatchModel>();

            var query = from p in db.Profiles.AsNoTracking()
                        where p.Gender == gender && p.LocationId1 == locId1
                        select p;

            var myAnswers = _matchCalc.GetUserAnswers(profileId);

            foreach (var p in query)
            {
                var pctMatch = _matchCalc.CalculateMatchPercent(p.Id, myAnswers);

                matches.Add(new MatchModel
                {
                    MatchPercent = pctMatch,
                    Name = p.Name,
                    ProfileId = p.Id
                });
            }

            return matches;
        }
    }
}
