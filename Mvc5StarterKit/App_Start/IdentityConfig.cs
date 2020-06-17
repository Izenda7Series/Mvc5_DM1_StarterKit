using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Mvc5StarterKit.Models;
using System;
using System.Data.Entity;
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
