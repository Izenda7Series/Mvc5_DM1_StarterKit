using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mvc5StarterKit.Controllers
{
    public class ReportController : Controller
    {
        // show report Viewer by id
        public ActionResult ReportViewer(string id)
        {
            ViewBag.Id = id;
            return View();
        }


        public ActionResult ReportCustomFilterViewer()
        {
            return View();
        }

        public ActionResult ReportParts()
        {
            return View();
        }

        public ActionResult AdvancedReportParts()
        {
            return View();
        }
    }
}