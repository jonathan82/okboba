﻿using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using okboba.Repository.Models;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using okboba.Resources;

namespace okboba.Repository.RedisRepository
{
    /// <summary>
    /// The class contains functions to save and retrieve match results from the Redis cache.
    /// The list of matches for each user is stored in a key with the following format:
    /// 
    ///   - matches:[profileId]         ex. matches:3252
    ///   - matchcriteria:[profileId]   ex. matchcriteria:3252 => human readable search string
    ///   
    /// First the matchcriteria is retrieved and compared with the given criteria, if they match
    /// then the list of matches is retrieved, otherwise save the new matches and criteria.
    /// </summary>
    public class SXMatchRepository : IRedisMatchRepository
    {
        #region Singelton
        private static SXMatchRepository _instance;
        private static SXGenericRepository _redis;

        //private ConnectionMultiplexer _redis;

        private SXMatchRepository()
        {
            _redis = SXGenericRepository.Instance;
        }

        public static SXMatchRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SXMatchRepository();
                }
                return _instance;
            }
        }
        #endregion
    

        //////////////////////// Private Methods /////////////////////////////////
        private string KeyMatches(int profileId)
        {
            return "matches:" + profileId;
        }

        private string KeyCriteria(int profileId)
        {
            return "matchcriteria:" + profileId;
        }

        private string CriteriaString(MatchCriteriaModel criteria)
        {
            string val = "";
            val += "gender:" + criteria.Gender;
            val += ":locationid1:" + criteria.LocationId1;
            return val;
        }

        private bool ExistsAndMeetsCriteria(int profileId, MatchCriteriaModel criteria, IDatabase db)
        {
            //check if criteria matches the one given
            var cachedCriteria = (string)db.StringGet(KeyCriteria(profileId));

            if (cachedCriteria == null || cachedCriteria != CriteriaString(criteria))
            {
                return false;
            }

            //check if matches exist in cache
            var key = KeyMatches(profileId);
            if (!db.KeyExists(key))
            {
                return false;
            }

            return true;
        }

        private byte[] Serialize(object o)
        {
            if (o == null)
            {
                return null;
            }
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, o);
                byte[] objectDataAsStream = memoryStream.ToArray();
                return objectDataAsStream;
            }
        }

        private T Deserialize<T>(byte[] stream)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            if (stream == null)
                return default(T);

            using (MemoryStream memoryStream = new MemoryStream(stream))
            {
                T result = (T)binaryFormatter.Deserialize(memoryStream);
                return result;
            }
        }


        ///////////////////// Public Methods /////////////////////////////////

        /// <summary>
        /// Gets a random subset of numReturn matches out of numConsidered from the cache for
        /// the given user. Makes sure the matches meet the given criteria otherwise
        /// returns null. Similar to "Get".
        /// </summary>        
        public IList<MatchModel> Recommended(int profileId, MatchCriteriaModel criteria, int numReturn, int numConsidered)
        {
            var db = _redis.GetDatabase();
            var rand = new Random();

            if (!ExistsAndMeetsCriteria(profileId, criteria, db))
            {
                return null;
            }

            var key = KeyMatches(profileId);

            var length = db.ListLength(key);

            numConsidered = numConsidered > length ? (int)length : numConsidered;

            var list = new List<MatchModel>();

            for (int i = 0; i < numReturn; i++)
            {
                var next = rand.Next(numConsidered);
                var match = db.ListGetByIndex(key, next);
                list.Add(Deserialize<MatchModel>(match));
            }

            return list;    
        }

        /// <summary>
        /// Saves the given matches to the cache as well as the match criteria
        /// Sets an expiration for the keys.
        /// </summary>
        public void Save(int profileId, IList<MatchModel> matches, MatchCriteriaModel criteria)
        {
            var db = _redis.GetDatabase();

            var key = KeyMatches(profileId);
            var keyCriteria = KeyCriteria(profileId);

            //clear the existing matches first
            db.KeyDelete(key);

            //Save the matches to a Redis list
            foreach (var match in matches)
            {
                db.ListRightPush(key, Serialize(match));
            }

            db.StringSet(keyCriteria, CriteriaString(criteria));

            db.KeyExpire(key, DateTime.Now.AddMinutes(OkbConstants.EXPIRE_MATCHES));
            db.KeyExpire(keyCriteria, DateTime.Now.AddMinutes(OkbConstants.EXPIRE_MATCHES));
        }

        /// <summary>
        /// Gets the list of matches from the cache for the given profile Id. 
        /// 
        ///  - Redis list commands will return empty list if the start and end values are 
        ///    out of range so we don't need to check for valid page value.
        ///  - Checks if the match criteria saved against the one given and only retrieves matches
        ///    if they're the same.
        ///  - Returns null if matches don't exist in cache or match criteria differ
        ///  - Returns empty list for out of range page values
        /// </summary>
        public IList<MatchModel> Get(int profileId, MatchCriteriaModel criteria, int page = 1)
        {
            var db = _redis.GetDatabase();

            //check if criteria matches the one given
            var cachedCriteria = (string)db.StringGet(KeyCriteria(profileId));
            if (cachedCriteria==null || cachedCriteria != CriteriaString(criteria))
            {
                return null;
            }

            //check if matches exist in cache
            var key = KeyMatches(profileId);
            if(!db.KeyExists(key))
            {
                return null;
            }

            int start = (page - 1) * OkbConstants.MATCHES_PER_PAGE,
                end = start + OkbConstants.MATCHES_PER_PAGE - 1;

            var array = db.ListRange(key, start, end);
            var list = new List<MatchModel>();

            foreach (var match in array)
            {
                list.Add(Deserialize<MatchModel>(match));
            }

            return list;
        }
    }
}
