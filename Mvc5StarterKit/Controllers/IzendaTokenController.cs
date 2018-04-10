using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Mvc5StarterKit.Controllers
{
    /// <summary>
    /// The MVC Controller provides route for generating Izenda token from logged-in user.
    /// The api indisde this controller is mostly using internally in this integrated application.
    /// </summary>
    [Authorize]
    public class IzendaTokenController : Controller
    {
        /// <summary>
        /// Generate Izenda token from current logged-in user.
        /// </summary>
        /// <returns>Return encrypted token string of username and unique tenant name from current logged-in user</returns>
        [HttpGet]
        [Authorize]
        public ActionResult GenerateToken()
        {
            var username = User.Identity.GetUserName();
            var tenantName = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirstValue("tenantName");


            var user = new Models.AccessTokenInfo { UserName = username, TenantUniqueName = tenantName };
            var token = IzendaBoundary.IzendaTokenAuthorization.GetToken(user);
            return Json(new { token = token }, JsonRequestBehavior.AllowGet);
        }
    }
}