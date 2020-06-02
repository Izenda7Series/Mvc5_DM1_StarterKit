using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Mvc5StarterKit.Models;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Mvc5StarterKit.ApiControllers
{
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        #region Variables
        private ApplicationUserManager _userManager;
        #endregion

        #region Properties
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        } 
        #endregion

        #region CTOR
        public AccountController()
        { }

        public AccountController(ApplicationUserManager userManager, ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
        }
        #endregion

        #region Methods
        [HttpGet]
        [Route("validateIzendaAuthToken")]
        public UserInfo ValidateIzendaAuthToken(string access_token)
        {
            var userInfo = IzendaBoundary.IzendaTokenAuthorization.GetUserInfo(access_token);
            return userInfo;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetIzendaAccessToken")]
        public IHttpActionResult GetIzendaAccessToken(string message)
        {
            var userInfo = IzendaBoundary.IzendaTokenAuthorization.DecryptIzendaAuthenticationMessage(message);
            var token = IzendaBoundary.IzendaTokenAuthorization.GetToken(userInfo);

            return Ok(new { Token = token });
        }
        #endregion

        #region Helpers
        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
        #endregion
    }
}
