using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository.EntityRepository
{
    public class EntityProfileRepository : IProfileRepository
    {
        #region Singleton
        private static EntityProfileRepository instance;
        private EntityProfileRepository() { }

        public static EntityProfileRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EntityProfileRepository();
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
