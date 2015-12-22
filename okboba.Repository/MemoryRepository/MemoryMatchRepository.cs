using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using okboba.Repository;
using okboba.Repository.Models;
using okboba.Entities;
using okboba.MatchCalculator;
using okboba.Resources;

namespace okboba.Repository.MemoryRepository
{

    public class MemoryMatchRepository : IMatchRepository
    {
        #region Singelton
        private static MemoryMatchRepository instance;
        private MemoryMatchRepository()
        {
            _matchCalc = MatchCalc.Instance;
        }

        public static MemoryMatchRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MemoryMatchRepository();
                }
                return instance;
            }
        }
        #endregion

        //////////////////// Member variables /////////////////
        private MatchCalc _matchCalc;
        const int MAX_MATCH_RESULTS = 3000;

        private IQueryable<Profile> BuildSearchQuery(OkbDbContext db, MatchCriteriaModel criteria)
        {
            var query = from p in db.Profiles.AsNoTracking()
                        select p;

            if (!(criteria.Gender == OkbConstants.UNKNOWN_GENDER))
            {
                query = query.Where(p => p.Gender == criteria.Gender);
            }
            if (criteria.LocationId1 != 0)
            {
                query = query.Where(p => p.LocationId1 == criteria.LocationId1);
            }

            //Limit to 4000 matches
            query = query.Take(MAX_MATCH_RESULTS);

            return query;
        }

        /// <summary>
        /// Peforms a match search for a user given their search preferences and returns a list
        /// of sorted matches.
        /// </summary>
        public List<MatchModel> MatchSearch(int profileId, MatchCriteriaModel criteria)
        {
            var db = new OkbDbContext();
            var matches = new List<MatchModel>();

            var query = BuildSearchQuery(db, criteria);

            var myAnswers = _matchCalc.GetAnswerDict(profileId);

            foreach (var p in query)
            {
                var matchResult = _matchCalc.CalculateMatchPercent(p.Id, myAnswers);

                matches.Add(new MatchModel
                {
                    MatchPercent = matchResult.MatchPercent,
                    FriendPercent = matchResult.FriendPercent,
                    EnemyPercent = matchResult.EnemeyPercent,
                    Name = p.Nickname,
                    ProfileId = p.Id,
                    Photo = p.GetFirstHeadshot(),
                    Age = DateTime.Today.Year - p.Birthdate.Year,
                    Gender = p.Gender
                });
            }

            //For now just sort the list by match %
            matches.Sort(delegate (MatchModel m1, MatchModel m2)
            {
                return m2.MatchPercent.CompareTo(m1.MatchPercent);
            });

            return matches;
        }

        /// <summary>
        /// Calculates match between two users
        /// </summary>
        public MatchModel CalculateMatch(int profileId1, int profileId2)
        {
            var ans1 = _matchCalc.GetAnswerDict(profileId1);
            var result = _matchCalc.CalculateMatchPercent(profileId2, ans1);
            return new MatchModel
            {
                MatchPercent = result.MatchPercent,
                FriendPercent = result.FriendPercent,
                EnemyPercent = result.EnemeyPercent
            };
        }
    }
}
