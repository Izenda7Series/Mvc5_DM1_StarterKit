using System.Web.Mvc;

namespace Mvc5StarterKit.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DashboardDesigner()
        {
            return View();
        }

        // GET: DashboardViewer
        public ActionResult DashboardViewer(string id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}