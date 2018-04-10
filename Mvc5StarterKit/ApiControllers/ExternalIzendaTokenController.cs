using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Mvc5StarterKit.ApiControllers
{
    /// <summary>
    /// The controller provides api for getting izenda token from outside of integrated application.
    /// Izenda CopyConsole tool uses api path api/User/GenerateToken for getting izenda token to handle communcation with Izenda API Service.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/User")]
    public class ExternalIzendaTokenController : ApiController
    {
        /// <summary>
        /// Generate Izenda token from current logged-in user.
        /// Izenda token is encrypted from username and unique tenant name.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GenerateToken")]
        public string GenerateToken()
        {
            var username = User.Identity.GetUserName();
            var tenantName = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirstValue("tenant");

            var user = new Models.AccessTokenInfo { UserName = username, TenantUniqueName = tenantName };
            var token = IzendaBoundary.IzendaTokenAuthorization.GetToken(user);

            return token;
        }
    }
}
