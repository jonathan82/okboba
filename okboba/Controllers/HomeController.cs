using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using okboba.Resources;

namespace okboba.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        { 
            return View();
        }

    }
}