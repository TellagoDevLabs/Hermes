using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Web.Infrastructure;

[assembly: WebActivator.PostApplicationStartMethod(typeof(TellagoStudios.Hermes.RestService.AppStart_RegisterRoutesAreasFilters), "Start")]

namespace TellagoStudios.Hermes.RestService {
    public static class AppStart_RegisterRoutesAreasFilters {
        public static void Start() {
            // Set everything up with you having to do any work.
            // I'm doing this because it means that
            // your app will just run. You might want to get rid of this 
            // and integrate with your own Global.asax. 
            // It's up to you. 
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "admin/{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            //Special Route for css so that images are relative to it
            routes.MapRoute("css",
                           "public/css/{filename}.css",
                           new { controller = "Css", action = "Index" },
                           new[] { "TellagoStudios.Hermes.RestService.Controllers" });
        }
    
    }

}