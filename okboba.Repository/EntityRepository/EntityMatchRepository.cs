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

namespace okboba.Repository.EntityRepository
{

    public class EntityMatchRepository : IMatchRepository
    {
        #region Singelton
        private static EntityMatchRepository instance;
        private EntityMatchRepository()
        {
            _matchCalc = MatchCalc.Instance;
            _locRepo = EntityLocationRepository.Instance;
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
        private ILocationRepository _locRepo;
        

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

            //Limit maximum match results returned
            query = query.Take(OkbConstants.MAX_MATCH_RESULTS);

            return query;
        }

        /// <summary>
        /// Peforms a match search for a user given their search preferences and returns a list
        /// of sorted matches.
        /// </summary>
        public List<MatchModel> Search(int profileId, MatchCriteriaModel criteria)
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
                    UserId = p.UserId,
                    Nickname = p.Nickname,
                    ProfileId = p.Id,
                    Photo = p.GetFirstHeadshot(),
                    Age = p.GetAge(),
                    Gender = p.Gender,
                    Location = _locRepo.GetLocationString(p.LocationId1, p.LocationId2)
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
        public MatchModel Calculate(int profileId1, int profileId2)
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
