﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Mvc5StarterKit.Controllers
{
    public class HomeController : Controller
    {
        #region Base Actions
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region Izenda Actions
        [Route("izenda/settings")]
        [Route("izenda/new")]
        [Route("izenda/dashboard")]
        [Route("izenda/report")]
        [Route("izenda/reportviewer")]
        [Route("izenda/reportviewerpopup")]
        [Route("izenda")]
        public ActionResult Izenda()
        {
            return View();
        }

        [Authorize]
        public ActionResult Settings()
        {
            return View();
        }

        [Authorize]
        public ActionResult Reports()
        {
            return View();
        }

        [Authorize]
        public ActionResult ReportDesigner()
        {
            return View();
        }

        [Authorize]
        public ActionResult Dashboards()
        {
            return View();
        }

        [Authorize]
        public ActionResult DashboardDesigner()
        {
            return View();
        }

        [Authorize]
        public ActionResult ReportPart(Guid id, string token)
        {
            ViewBag.Id = id;
            ViewBag.Token = token;
            return View();
        }

        [Authorize]
        public ActionResult IframeViewer(string id)
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
        #endregion
    }
}