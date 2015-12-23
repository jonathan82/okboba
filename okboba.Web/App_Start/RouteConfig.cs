﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace okboba
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "StaticPage",
                url: "static/{page}",
                defaults: new { controller = "Static", action = "Page" }
            );

            routes.MapRoute(
                name: "IndexWithId",
                url: "{controller}/{userId}",
                defaults: new { action = "Index", userId = UrlParameter.Optional  },
                constraints: new { userId = @"[a-zA-Z0-9]+" }
            );

            routes.MapRoute(
                name: "Index",
                url: "{controller}/{action}/{id}",
                defaults: new { action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Register", id = UrlParameter.Optional }
            );
        }
    }
}
