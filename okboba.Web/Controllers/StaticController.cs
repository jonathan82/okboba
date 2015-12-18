using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace okboba.Web.Controllers
{
    public class StaticController : Controller
    {
        // GET: Static
        public ActionResult Page(string page)
        {
            return View(page);
        }
    }
}