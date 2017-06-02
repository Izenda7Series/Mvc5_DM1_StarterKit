using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Mvc5StarterKit.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        [Authorize]
        public ActionResult GenerateToken()
        {
            var username = User.Identity.GetUserName();
            var tenantName = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirstValue("tenantName");


            var user = new Models.UserInfo { UserName = username, TenantUniqueName = tenantName };
            var token = IzendaBoundary.IzendaTokenAuthorization.GetToken(user);
            return Json(new { token = token }, JsonRequestBehavior.AllowGet);
        }

    }
}