using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mvc5StarterKit.Managers
{
    public class TenantManager
    {
        #region Methods
        public Models.Tenant GetTenantByName(string name)
        {
            using (var context = Models.ApplicationDbContext.Create())
            {
                var tenant = context.Tenants.Where(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();

                return tenant;
            }
        }

        public IEnumerable<string> GetAllTenant()
        {
            using (var context = Models.ApplicationDbContext.Create())
            {
                var tenantList = new List<string>();

                foreach (var tenant in context.Tenants)
                {
                    tenantList.Add(tenant.Name);
                }

                return tenantList;
            }
        }

        public async Task<Models.Tenant> SaveTenantAsync(Models.Tenant tenant)
        {
            using (var context = Models.ApplicationDbContext.Create())
            {
                context.Tenants.Add(tenant);
                await context.SaveChangesAsync();

                return tenant;
            }
        } 
        #endregion
    }
}