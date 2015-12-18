using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using okboba.MatchCalculator;
using System.Configuration;
using okboba.Repository.RedisRepository;
using System.Diagnostics;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace okboba.MatchApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Load the answer cache
            var timer = new Stopwatch();
            timer.Start();
            var numAnswersLoaded = MatchCalc.Instance.LoadAnswerCache();
            timer.Stop();            
            log.Info(string.Format("{0} answers loaded in {1} s", numAnswersLoaded, timer.ElapsedMilliseconds / 1000));

            // Setup Redis repository
            var redisConnStr = ConfigurationManager.ConnectionStrings["RedisConnectionString"].ConnectionString;
            var matchResultsTTL = ConfigurationManager.AppSettings["MatchResultsTTL"];
            RedisMatchRepository.Instance.RedisConnectionString = redisConnStr;
            RedisMatchRepository.Instance.MatchResultsTTL = Convert.ToInt32(matchResultsTTL);
        }
    }
}
