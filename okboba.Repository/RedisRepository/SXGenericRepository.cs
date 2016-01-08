using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository.RedisRepository
{
    /// <summary>
    /// This class encapsulates a generic singleton Redis store.  It contains the connection object
    /// to Redis that should be shared among other repositories.  It also exposes some Redis convenience
    /// functions to save and retrieve objects.
    /// </summary>
    public class SXGenericRepository
    {
        #region Singelton
        private static SXGenericRepository _instance;
        private ConnectionMultiplexer _redis;

        private SXGenericRepository(string connString)
        {
            _redis = ConnectionMultiplexer.Connect(connString);
        }

        public static SXGenericRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new Exception("Object SXGenericRepository not created");
                }
                return _instance;
            }
        }

        public static void Create(string connString)
        {
            if (_instance != null)
            {
                throw new Exception("Object SXGenericRepository already created");
            }
            _instance = new SXGenericRepository(connString);
        }
        #endregion

        public IDatabase GetDatabase()
        {
            return _redis.GetDatabase();
        }

        //public void StringSet(string key, RedisValue val)
        //{
        //    var db = _redis.GetDatabase();
        //    db.StringSet(key, val);
        //}

        //public RedisValue StringGet(string key)
        //{
        //    var db = _redis.GetDatabase();
        //    return db.StringGet(key);
        //}

        //public void Increment(string key)
        //{
        //    var db = _redis.GetDatabase();
        //    db.StringIncrement(key);
        //}

        //public void Decrement(string key)
        //{
        //    var db = _redis.GetDatabase();
        //    db.StringDecrement(key);
        //}

        /// <summary>
        /// Save an object in the cache. Calls the extension method which serializes the object first.
        /// </summary>
        public void Set(string key, object value)
        {
            var db = _redis.GetDatabase();
            db.Set(key, value);
        }

        /// <summary>
        /// Gets and object from the cache. Calls the extension method which deserializes the object.
        /// </summary>        
        public object Get(string key)
        {
            var db = _redis.GetDatabase();
            return db.Get(key);
        }
    }
}
