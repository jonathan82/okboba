using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using okboba.Entities;

namespace okboba.Repository.EntityRepository
{
    public class EntityLocationRepository : ILocationRepository
    {
        #region Singelton
        private static EntityLocationRepository instance;
        private EntityLocationRepository() { }

        public static EntityLocationRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EntityLocationRepository();
                }
                return instance;
            }
        }
        #endregion

        public List<Location> GetProvinceList()
        {
            var db = new OkbDbContext();

            return db.Locations
                .AsEnumerable()
                .Distinct()
                .OrderBy(loc => loc.LocationId1)
                .ToList();
        }

        public List<Location> GetDistrictList(int id)
        {
            var db = new OkbDbContext();

            var result = from loc in db.Locations
                         where loc.LocationId1 == id
                         orderby loc.LocationId2
                         select loc;

            return result.ToList();
        }
    }
}
