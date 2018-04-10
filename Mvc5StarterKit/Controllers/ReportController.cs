using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mvc5StarterKit.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReportDesigner()
        {
            return View();
        }

        // show report Viewer by id
        public ActionResult ReportViewer(string id)
        {
            ViewBag.Id = id;
            return View();
        }

        [AllowAnonymous]
        public ActionResult ReportPart(Guid id, string token)
        {
            ViewBag.Id = id;
            ViewBag.Token = token;
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