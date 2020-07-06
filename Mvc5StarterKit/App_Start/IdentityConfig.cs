using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Mvc5StarterKit.Models;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mvc5StarterKit
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        #region CTOR
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }
        #endregion

        #region Methods
        public async Task<ApplicationUser> FindTenantUserAsync(string tenant, string username, string password)
        {
            var passwordStore = Store as IUserPasswordStore<ApplicationUser>;

            var context = ApplicationDbContext.Create();

            var query = context.Users
                .Include(x => x.Tenant)
                .Where(x => x.UserName.Equals(username, StringComparison.InvariantCultureIgnoreCase));

            if (string.IsNullOrWhiteSpace(tenant))
                query = query.Where(x => !x.Tenant_Id.HasValue);
            else
                query = query.Where(x => x.Tenant.Name.Equals(tenant, StringComparison.InvariantCultureIgnoreCase));

            var user = await query
                .SingleOrDefaultAsync();

            if (await CheckPasswordAsync(user, password))
                return user;

            return null;
        }

        /// <summary>
        /// Overload FindTenantUserAsync. In case of Active Directory authentication, password is not required for finding tenant
        /// </summary>
        public async Task<ApplicationUser> FindTenantUserAsync(string tenant, string username)
        {
            var passwordStore = Store as IUserPasswordStore<ApplicationUser>;

            var context = ApplicationDbContext.Create();

            var query = context.Users
                .Include(x => x.Tenant)
                .Where(x => x.UserName.Equals(username, StringComparison.InvariantCultureIgnoreCase));

            if (string.IsNullOrWhiteSpace(tenant))
                query = query.Where(x => !x.Tenant_Id.HasValue);
            else
                query = query.Where(x => x.Tenant.Name.Equals(tenant, StringComparison.InvariantCultureIgnoreCase));

            var user = await query.SingleOrDefaultAsync();

            return user;
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        } 
        #endregion
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        #region CTOR
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
           : base(userManager, authenticationManager)
        {
        }
        #endregion

        #region Methods
        public async Task<bool> PasswordSigninAsync(string tenant, string username, string password, bool remember)
        {
            var user = await (UserManager as ApplicationUserManager).FindTenantUserAsync(tenant, username, password);

            if (user != null)
            {
                await SignInAsync(user, remember, true);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Login with Active Directory information.
        /// Please refer to the following link to get more information on Active Directory 
        /// https://docs.microsoft.com/en-us/windows-server/identity/ad-ds/get-started/virtual-dc/active-directory-domain-services-overview
        /// </summary>
        public async Task<bool> ADSigninAsync(string tenant, string password, bool remember)
        {
            var userName = Environment.UserName;
            var userDomainName = Environment.UserDomainName;
            var authenticationType = ContextType.Domain;
            UserPrincipal userPrincipal = null;
            bool isAuthenticated = false;

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userDomainName))
            {
                //CheckAllActiveDirectoryUser(); // only for debugging purpose

                using (var context = new PrincipalContext(authenticationType, Environment.UserDomainName))
                {
                    try
                    {
                        userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userName);

                        if (userPrincipal != null)
                        {
                            var email = userPrincipal.EmailAddress;

                            // Validate credential with Active Directory information. This is optional for authentication process.
                            // If you want check password one more time, you can check here. Otherwise, you need to remove password from parameter and skip this and set isAuthencate as true since userPrincipal is not null.
                            isAuthenticated = context.ValidateCredentials(userName, password, ContextOptions.Negotiate);

                            if (isAuthenticated)
                            {
                                using (var appUserManager = UserManager as ApplicationUserManager)
                                {
                                    // retrieve tenant information after validation process
                                    var user = await appUserManager.FindTenantUserAsync(tenant, email);

                                    if (user != null)
                                    {
                                        // now you can sign in with correct authticated user
                                        await SignInAsync(user, remember, true);

                                        return true;
                                    }
                                    else
                                        return false;
                                }
                            }
                            else
                                return false;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                        return false;
                    }

                    if (!isAuthenticated)
                        return false;

                    if (userPrincipal.IsAccountLockedOut())
                        return false;

                    if (userPrincipal.Enabled.HasValue && !userPrincipal.Enabled.Value)
                        return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Check list of Active Directory users
        /// </summary>
        private void CheckAllActiveDirectoryUser()
        {
            using (var context = new PrincipalContext(ContextType.Domain, Environment.UserDomainName))
            using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
            {
                foreach (var result in searcher.FindAll())
                {
                    DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
                    Debug.WriteLine("First Name: " + de.Properties["givenName"].Value);
                    Debug.WriteLine("Last Name : " + de.Properties["sn"].Value);
                    Debug.WriteLine("SAM account name   : " + de.Properties["samAccountName"].Value);
                    Debug.WriteLine("User principal name: " + de.Properties["userPrincipalName"].Value);
                }
            }
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        } 
        #endregion
    }

    // Configure the application role manager used in this application. RoleManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationRoleManager : RoleManager<IdentityRole, string>
    {
        #region CTOR
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> store)
           : base(store)
        {
        }
        #endregion

        #region Methods
        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var manager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
            return manager;
        } 
        #endregion
    }
}
