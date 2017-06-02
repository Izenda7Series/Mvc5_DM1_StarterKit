using System.Web.Mvc;

namespace Mvc5StarterKit.Controllers
{
    public class DashboardController : Controller
    {
        // GET: DashboardViewer
        public ActionResult DashboardViewer(string id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}