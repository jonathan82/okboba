using okboba.App_Start;
using okboba.Repository.RedisRepository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace okboba
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            OkbobaConfig.Init();

            //Increase performance by disabling WebForms view engine
            //http://encosia.com/a-harsh-reminder-about-the-importance-of-debug-false/
            //ViewEngines.Engines.Clear();
            //ViewEngines.Engines.Add(new RazorViewEngine());

            // Create singleton for Redis connection object
            var redisConnStr = ConfigurationManager.ConnectionStrings["RedisConnectionString"].ConnectionString;
            SXGenericRepository.Create(redisConnStr);
        }
    }
}
