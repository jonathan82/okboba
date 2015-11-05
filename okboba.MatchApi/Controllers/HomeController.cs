using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace okboba.MatchApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                ViewBag.Title = "Home Page - authenticated";
            else
                ViewBag.Title = "Home Page - anonymous";

            return View();
        }
    }
}
