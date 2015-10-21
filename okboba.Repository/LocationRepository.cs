using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository
{
    public class LocationRepository
    {
        #region Singelton
        private static LocationRepository instance;
        private LocationRepository() { }

        public static LocationRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LocationRepository();
                }
                return instance;
            }
        }

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
        #endregion
    }
}
