using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Mvc5StarterKit.Controllers
{
    public class DashboardController : Controller
    {
        #region Methods
        // GET: DashboardViewer
        public ActionResult DashboardViewer(string id)
        {
            var queryString = Request.QueryString;
            dynamic filters = new System.Dynamic.ExpandoObject();
            foreach (string key in queryString.AllKeys)
            {
                ((IDictionary<string, object>)filters).Add(key, queryString[key]);
            }

            ViewBag.Id = id;
            ViewBag.filters = JsonConvert.SerializeObject(filters);
            return View();
        } 
        #endregion
    }
}