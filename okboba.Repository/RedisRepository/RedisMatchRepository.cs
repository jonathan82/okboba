using okboba.Repository.Models;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository.RedisRepository
{
    public class RedisMatchRepository
    {
        #region Singelton
        private static RedisMatchRepository instance;
        private RedisMatchRepository() { }

        public static RedisMatchRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RedisMatchRepository();
                }
                return instance;
            }
        }
        #endregion

        ////////////////////// Member variables ///////////////////////////
        private IRedisClientsManager _redisClientsManager;
        public string RedisConnectionString
        {
            set
            {
                _redisClientsManager = new RedisManagerPool(value);
            }
        }
        public int MatchResultsTTL { get; set; } //seconds

        /////////////////////// Methods /////////////////////////////
        /// <summary>
        /// Save the list of match results to the cache with key being a combination of the
        /// profileId and search criteria. 
        /// </summary>
        public void SaveMatchResults(string key, List<MatchModel> results)
        {
            var client = _redisClientsManager.GetClient().As<MatchModel>();

            client.RemoveEntry(key);

            var matchList = client.Lists[key];

            foreach (var match in results)
            {
                matchList.Add(match);
            }

            //Expire in 5 min
            client.ExpireEntryIn(key, new TimeSpan(0,0,MatchResultsTTL));
        }

        /// <summary>
        /// Get the number of matches in the cache.
        /// </summary>
        public int GetMatchCount(string key)
        {
            var client = _redisClientsManager.GetClient();
            return (int)client.GetListCount(key);
        }

        /// <summary>
        /// Get a range of matches as a list of JSON strings with start and end indexes.
        /// </summary>        
        public List<string> GetMatches(string key, int start, int end)
        {
            var client = _redisClientsManager.GetClient();
            return client.GetRangeFromList(key, start, end-1); //ending at
        }

        /// <summary>
        /// Returns true if the key exists in the cache, false otherwise.
        /// </summary>
        public bool HasMatches(string key)
        {           
            var client = _redisClientsManager.GetClient();
            return client.ContainsKey(key);
        }

        /// <summary>
        /// Generates a string key for the match list to use in Redis based on the profileId
        /// and the given criteria.
        /// </summary>
        public string FormatKey(int profileId, MatchCriteriaModel criteria)
        {
            var key = profileId.ToString() + ":";
            key += criteria.Gender + ":";
            key += criteria.LocationId1.ToString();
            return key;
        }
    }
}
