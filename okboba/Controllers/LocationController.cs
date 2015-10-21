using okboba.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace okboba.Controllers
{
    public class LocationController : Controller
    {
        private LocationRepository locationRepo;

        public LocationController()
        {
            this.locationRepo = LocationRepository.Instance;
        }

        // GET: Location
        public JsonResult GetDistrictJson(int provinceId)
        {
            var locations = locationRepo.GetDistrictList(provinceId);
            var districtList = new List<object>();

            foreach (var loc in locations)
            {
                districtList.Add(new { id = loc.LocationId2, name = loc.LocationName2 });
            }

            return Json(districtList, JsonRequestBehavior.AllowGet);
        }
    }
}