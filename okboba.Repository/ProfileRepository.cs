using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository
{
    public class ProfileRepository
    {
        #region Singleton

        private static ProfileRepository instance;
        private ProfileRepository() { }

        public static ProfileRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProfileRepository();
                }
                return instance;
            }
        }
        #endregion

        public Profile GetProfile(int profileId)
        {
            var db = new OkbDbContext();
            var profile = db.Profiles.Find(profileId);
            return profile;
        }
    }
}
