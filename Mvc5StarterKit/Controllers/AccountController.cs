using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Mvc5StarterKit.IzendaBoundary;
using Mvc5StarterKit.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace Mvc5StarterKit.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region Variables
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        private static readonly string _defaultTenantFailureMessage = "Can't create a new tenant.";
        private static readonly string _defaultUserFailureMessage = "Can't create a new user.";
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

            bool result;
            bool.TryParse(ConfigurationManager.AppSettings["useADlogin"], out bool useADlogin);

            if (useADlogin && !string.IsNullOrEmpty(model.Tenant)) // if tenant is null, then assume that it is system level login. Go to the ValidateLogin which is used for regular login process first
            {
                result = await SignInManager.ADSigninAsync(model.Tenant, model.Password, model.RememberMe);
            }
            else
            { 
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                result = await SignInManager.PasswordSigninAsync(model.Tenant, model.Email, model.Password, model.RememberMe);
            }

            if (result)
                return RedirectToLocal(returnUrl);
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
        }

        // POST: /Account/CreateUser
        /// <summary>
        /// This example returns view that handles user creation to check values passed after submit. 
        /// You can customize your result view showing simple success messages or failure message.
        /// </summary>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(CreateUserViewModel model, string returnUrl)
        {
            var izendaAdminAuthToken = IzendaTokenAuthorization.GetIzendaAdminToken();
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

                var result = await UserManager.CreateAsync(user); // Save new user into client DB

                if (result.Succeeded) // if successful, then start creating a user at Izenda DB
                {
                    var assignedRole = !string.IsNullOrEmpty(model.SelectedRole) ? model.SelectedRole : "Employee";// set default role if required. As an example, Employee is set by default

                    if (!RoleManager.RoleExists(assignedRole)) // check assigned role exist in client DB. if not, assigned role is null
                    {
                        try
                        {
                            await RoleManager.CreateAsync(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole(assignedRole));
                            result = await UserManager.AddToRoleAsync(user.Id, assignedRole);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e);
                        }
                    }

                    if (result.Succeeded)
                    {
                        user.Tenant = IzendaUtilities.GetTenantByName(model.SelectedTenant); // set client DB application user's tenant

                        // Create a new user at Izenda DB
                        var success = await IzendaUtilities.CreateIzendaUser(
                            model.SelectedTenant,
                            model.UserID,
                            model.LastName,
                            model.FirstName,
                            model.IsAdmin,
                            assignedRole,
                            izendaAdminAuthToken);

                        if (success)
                            return RedirectToAction(returnUrl);
                        else
                            FailedUserCreateAction(_unknownFailureMessage);
                    }
                }
                else
                    FailedUserCreateAction(_defaultUserFailureMessage);

                AddErrors(result);
            }

            return FailedUserCreateAction(_defaultUserFailureMessage);
        }

        private ActionResult FailedUserCreateAction(string message)
        {
            TempData["WarningMessage"] = message;

            return RedirectToAction("FailedToCreateUser", "Account");
        }

        public ViewResult CreateUserSuccess()
        {
            // You can customize your ViewResult, we just display success message here
            ViewBag.Title = "Create User Success";

            return View();
        }

        public ViewResult FailedToCreateUser()
        {
            // You can customize your ViewResult, we just display success message here
            ViewBag.Title = "Create User Failure";

            return View();
        }

        // GET: /Account/CreateUser
        [Authorize]
        public ActionResult CreateUser()
        {
            ViewBag.Title = "Create User";

            var createUserViewModel = new CreateUserViewModel();
            var tenants = IzendaUtilities.GetAllTenants();

            createUserViewModel.Tenants = tenants;

            return View(createUserViewModel);
        }

        // POST: /Account/CreateTenant
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateTenant(CreateTenantViewModel model, string returnUrl)
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
                        var newTenant = new Tenant() { Name = model.TenantID };
                        await IzendaUtilities.SaveTenantAsync(newTenant);

                        return RedirectToAction(returnUrl);
                    }
                    else
                        // Izenda DB has the same tenant name. Display Message at CreateTenant.cshtml
                        return FailedTenantCreateAction(_defaultTenantFailureMessage);
                }
                else
                    // user DB has the same tenant name. Display Message at CreateTenant.cshtml
                    return FailedTenantCreateAction(_defaultTenantFailureMessage);
            }

            // If we got this far, something failed
            return FailedTenantCreateAction(_unknownFailureMessage);
        }

        private ActionResult FailedTenantCreateAction(string message)
        {
            TempData["WarningMessage"] = message; // custom message passed

            return RedirectToAction("FailedToCreateTenant", "Account");
        }

        public ViewResult CreateTenantSuccess()
        {
            // You can customize your ViewResult, we just display success message here
            ViewBag.Title = "Create Tenant Success";

            return View();
        }

        public ViewResult FailedToCreateTenant()
        {
            // You can customize your ViewResult, we just display success message here
            ViewBag.Title = "Create Tenant Failure";

            return View();
        }

        /// <summary>
        /// Get all roles from Izenda DB by selected tenant and return SelectedList for role selection dropdown list at view 
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<JsonResult> GetRoleListByTenant(string selectedTenant)
        {
            var selectList = new List<string>();
            var adminToken = IzendaTokenAuthorization.GetIzendaAdminToken();

            var izendaTenant = await IzendaUtilities.GetIzendaTenantByName(selectedTenant, adminToken);
            var roleDetailsByTenant = await IzendaUtilities.GetAllIzendaRoleByTenant(izendaTenant?.Id ?? null, adminToken);

            roleDetailsByTenant.ForEach(r => selectList.Add(r.Name));

            var itemList = selectList.Select(i => new SelectListItem { Text = i }).ToList();
            return Json(new SelectList(itemList, "Value", "Text"));
        }

        // GET: /Account/CreateUser
        [Authorize]
        public ActionResult CreateTenant()
        {
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
