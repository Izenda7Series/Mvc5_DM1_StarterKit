using Mvc5StarterKit.IzendaBoundary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mvc5StarterKit.IzendaBoundary
{
    public static class IzendaUtilities
    {
        #region Methods
        /// <summary>
        /// Create a tenant
        /// For more information, please refer to https://www.izenda.com/docs/ref/api_tenant.html#tenant-apis
        /// </summary>
        public static async Task<bool> CreateTenant(string tenantName, string tenantId, string authToken)
        {
            var existingTenant = await GetIzendaTenantByName(tenantName, authToken);
            if (existingTenant != null)
                return false;

            var tenantDetail = new TenantDetail
            {
                Active = true,
                Disable = false,
                Name = tenantName,
                TenantId = tenantId
            };

            // For more information, please refer to https://www.izenda.com/docs/ref/api_tenant.html#post-tenant
            return await WebApiService.Instance.PostTenantAsync("tenant", tenantDetail, authToken);
        }

        /// <summary>
        /// We are not supporting creating role here. TBD
        /// </summary>
        public static async Task<RoleDetail> CreateRole(string roleName, TenantDetail izendaTenant, string authToken)
        {
            var role = await GetIzendaRoleByTenantAndName(izendaTenant != null ? (Guid?)izendaTenant.Id : null, roleName, authToken);

            if (role == null)
            {
                role = new RoleDetail
                {
                    Active = true,
                    Deleted = false,
                    NotAllowSharing = false,
                    Name = roleName,
                    TenantId = izendaTenant != null ? (Guid?)izendaTenant.Id : null
                };

                var response = await WebApiService.Instance.PostReturnValueAsync<AddRoleResponeMessage, RoleDetail>("role", role, authToken);
                role = response.Role;
            }

            return role;
        }

        /// <summary>
        /// Adds the user to the Izenda database
        /// https://www.izenda.com/docs/ref/api_user.html?highlight=user%20integration#post-user-integration-saveuser
        /// </summary>
        public static async Task<bool> CreateIzendaUser(Mvc5StarterKit.Models.ApplicationUser appUser, string roleName, string authToken)
        {
            var izendaTenant = appUser.Tenant != null ? await GetIzendaTenantByName(appUser.Tenant.Name, authToken) : null;

            var izendaUser = new UserDetail
            {
                FirstName = izendaTenant != null ? izendaTenant.Name : string.Empty,
                LastName = appUser.UserName.Split('@')[0],
                Username = appUser.UserName,
                TenantDisplayId = izendaTenant != null ? izendaTenant.Name : string.Empty,
                TenantId = izendaTenant != null ? (Guid?)izendaTenant.Id : null,
                InitPassword = true,
                Active = false
            };

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                var izendaRole = await CreateRole(roleName, izendaTenant, authToken);
                izendaUser.Roles.Add(izendaRole);
            }

            bool success = await WebApiService.Instance.PostReturnValueAsync<bool, UserDetail>("user/integration/saveUser", izendaUser, authToken);

            return success;
        }

        private static async Task<RoleDetail> GetIzendaRoleByTenantAndName(Guid? tenantId, string roleName, string authToken)
        {
            var roles = await WebApiService.Instance.GetAsync<IList<RoleDetail>>("/role/all/" + (tenantId.HasValue ? tenantId.ToString() : null), authToken);

            if (roles != null)
                return roles.FirstOrDefault(r => r.Name.Equals(roleName, StringComparison.InvariantCultureIgnoreCase));

            return null;
        }

        private static async Task<TenantDetail> GetIzendaTenantByName(string tenantName, string authToken)
        {
            var tenants = await WebApiService.Instance.GetAsync<IList<TenantDetail>>("/tenant/allTenants", authToken);
            if (tenants != null)
                return tenants.FirstOrDefault(x => x.Name.Equals(tenantName, StringComparison.InvariantCultureIgnoreCase));

            return null;
        }
        #endregion
    }
}
