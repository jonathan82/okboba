using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using okboba.Entities;
using okboba.Repository.Models;
using Pinyin4net;

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

        public List<LocationPinyinModel> GetProvinceList()
        {
            var db = new OkbDbContext();

            var result = from loc in db.Locations.AsNoTracking()
                         group loc by loc.LocationId1 into loc1Group
                         select loc1Group.FirstOrDefault();

            var list = new List<LocationPinyinModel>();

            foreach (var loc in result)
            {
                var pinyinArray = PinyinHelper.ToHanyuPinyinStringArray(loc.LocationName1[0]);

                list.Add(new LocationPinyinModel
                {
                    LocationId = loc.LocationId1,
                    LocationName = loc.LocationName1,
                    Pinyin = pinyinArray != null ? pinyinArray[0] : ""
                });

                //Fix up zhong => chongqing
                if (list[list.Count - 1].LocationName == "重庆") list[list.Count - 1].Pinyin = "chong";
            }

            list.Sort((loc1, loc2) => loc1.Pinyin.CompareTo(loc2.Pinyin));

            return list;

            //return db.Locations
            //    .AsEnumerable()
            //    .Distinct()
            //    .OrderBy(loc => loc.LocationId1)
            //    .ToList();
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

        public string GetLocationString(int locId1, int locId2)
        {
            var db = new OkbDbContext();
            var loc = db.Locations.Find(locId1, locId2);
            if(loc != null)
            {
                return loc.LocationName1 + ", " + loc.LocationName2;
            }
            return "";
        }
    }
}
