using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Mvc5StarterKit.Controllers
{
    public class ReportController : Controller
    {
        #region Methods
        public ActionResult ReportViewer(string id)
        {
            var queryString = Request.QueryString;
            dynamic filters = new System.Dynamic.ExpandoObject();
            foreach (string key in queryString.AllKeys)
            {
                ((IDictionary<string, object>)filters).Add(key, queryString[key]);
            }

            ViewBag.Id = id;
            ViewBag.overridingFilterQueries = JsonConvert.SerializeObject(filters);
            return View();
        }

        public ActionResult ReportParts()
        {
            return View();
        }
        #endregion
    }
}
