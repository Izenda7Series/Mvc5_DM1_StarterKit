using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mvc5StarterKit
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("api/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "ReportPart",
                url: "izenda/viewer/reportpart/{id}",
                defaults: new { controller = "Home", action = "ReportPart" }
            );

            routes.MapRoute(
                name: "IframeViewer",
                url: "izenda/report/iframe/{id}",
                defaults: new { controller = "Home", action = "IframeViewer" }
            );

            routes.MapRoute(
                name: "ReportViewer",
                url: "izenda/report/view/{id}",
                defaults: new { controller = "Report", action = "ReportViewer" }
            );

            routes.MapRoute(
                name: "DashboardViewer",
                url: "izenda/dashboard/edit/{id}",
                defaults: new { controller = "Dashboard", action = "DashboardViewer" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
