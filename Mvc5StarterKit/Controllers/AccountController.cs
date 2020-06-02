using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Mvc5StarterKit.IzendaBoundary;
using Mvc5StarterKit.Models;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mvc5StarterKit.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region Variables
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        private static readonly string _defaultTenantFailureMessage = "Can't creat a new tenant. The tenant name or id already exists.";
        private static readonly string _defaultUserFailureMessage = "Can't create a new user. The user name or id already exists.";
        private static readonly string _unknownFailureMessage = "Server does not allow your request.";
        #endregion

        #region Properties
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        #endregion

        #region CTOR
        public AccountController()
        { }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }
        #endregion

        #region Methods
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSigninAsync(model.Tenant, model.Email, model.Password, model.RememberMe);
            if (result)
                return RedirectToLocal(returnUrl);
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
        }

        // POST: /Account/CreateUser
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(CreateUserViewModel model, string returnUrl = null)
        {
            model.Tenants = IzendaUtilities.GetAllTenants(); // prevent null exception when redirected
            
            if (ModelState.IsValid)
            {
                int? tenantId = null;

                if (model.SelectedTenant != null)
                {
                    tenantId = IzendaUtilities.GetTenantByName(model.SelectedTenant).Id;
                    model.IsAdmin = false;
                }

                var user = new ApplicationUser
                {
                    UserName = model.UserID,
                    Email = model.UserID,
                    Tenant_Id = tenantId,
                };

                var result = await UserManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    var assignedRole = "Employee"; // set default role if required

                    if (RoleManager.RoleExists(assignedRole))
                        result = await UserManager.AddToRoleAsync(user.Id, assignedRole);
                    else
                        assignedRole = null;

                    if (result.Succeeded)
                    {
                        var izendaAdminAuthToken = IzendaBoundary.IzendaTokenAuthorization.GetIzendaAdminToken();
                        user.Tenant = IzendaUtilities.GetTenantByName(model.SelectedTenant);

                        var success = await IzendaBoundary.IzendaUtilities.CreateIzendaUser(
                            model.SelectedTenant,
                            model.UserID,
                            model.LastName,
                            model.FirstName,
                            model.IsAdmin,
                            assignedRole, 
                            izendaAdminAuthToken);

                        if (success)
                        {
                            TempData["SuccessMessage"] = "User has been created successfully";
                            return View(model);
                        }
                        else
                            FailedUserCreateAction(model, _unknownFailureMessage);
                    }
                }
                else
                    FailedUserCreateAction(model, _defaultUserFailureMessage);

                AddErrors(result);
            }

            return FailedUserCreateAction(model, _defaultUserFailureMessage);
        }

        private ActionResult FailedUserCreateAction(CreateUserViewModel model, string message)
        {
            TempData["WarningMessage"] = message;
            return View(model);
        }

        // GET: /Account/CreateUser
        [AllowAnonymous]
        public ActionResult CreateUser()
        {
            ViewBag.ReturnUrl = null;
            ViewBag.Title = "Create User";

            var createUserViewModel = new CreateUserViewModel();
            var tenants = IzendaUtilities.GetAllTenants();

            createUserViewModel.Tenants = tenants;

            return View(createUserViewModel);
        }

        // POST: /Account/CreateTenant
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateTenant(CreateTenantViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var izendaAdminAuthToken = IzendaTokenAuthorization.GetIzendaAdminToken();
                var tenantName = model.TenantName;

                var isTenantExist = IzendaUtilities.GetTenantByName(tenantName); // check user DB first

                if (isTenantExist == null)
                {
                    // try to create a new tenant at izenda DB
                    var success = await IzendaUtilities.CreateTenant(tenantName, model.TenantID, izendaAdminAuthToken);

                    if (success)
                    {
                        // save a new tenant at user DB
                        var newTenant = new Tenant() { Name = tenantName };
                        await IzendaUtilities.SaveTenantAsync(newTenant);

                        TempData["SuccessMessage"] = "Tenant has been created successfully";
                        return View(model);
                    }
                    else
                        // Izenda DB has the same tenant name. Display Message at CreateTenant.cshtml
                        return FailedTenantCreateAction(model, _defaultTenantFailureMessage);
                }
                else
                    // user DB has the same tenant name. Display Message at CreateTenant.cshtml
                    return FailedTenantCreateAction(model, _defaultTenantFailureMessage);
            }

            // If we got this far, something failed, re-display form
            return FailedTenantCreateAction(model, _unknownFailureMessage);
        }

        private ActionResult FailedTenantCreateAction(CreateTenantViewModel model, string message)
        {
            TempData["WarningMessage"] = message;
            return View(model);
        }

        // GET: /Account/CreateUser
        [AllowAnonymous]
        public ActionResult CreateTenant()
        {
            ViewBag.ReturnUrl = null;
            ViewBag.Title = "Create Tenant";

            return View();
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Helpers
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}
