using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using okboba.Resources;
using okboba.Repository;
using Newtonsoft.Json;
using okboba.Repository.EntityRepository;

namespace okboba.Controllers
{
    
    [Authorize]
    public class HomeController : OkbBaseController
    {
        private ILocationRepository locationRepo;

        public HomeController()
        {
            this.locationRepo = EntityLocationRepository.Instance;
        }

        public ActionResult Index()
        {
            //var locationList = locationRepo.GetProvinceList();
            //var provinceList = new List<object>();                       

            //foreach (var loc in locationList)
            //{
            //    provinceList.Add(new { id = loc.LocationId1, name = loc.LocationName1 });
            //}

            //var json = JsonConvert.SerializeObject(provinceList);

            //ViewBag.JsonProvinces = json;

            return View();
        }

    }
}