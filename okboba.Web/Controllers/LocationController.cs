using okboba.Repository;
using okboba.Repository.EntityRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace okboba.Controllers
{
    public class LocationController : Controller
    {
        private ILocationRepository locationRepo;

        public LocationController()
        {
            this.locationRepo = EntityLocationRepository.Instance;
        }

        // GET: Location
        public JsonResult GetDistricts(int provinceId)
        {
            var districts = locationRepo.GetDistricts(provinceId);

            return Json(districts, JsonRequestBehavior.AllowGet);
        }
    }
}