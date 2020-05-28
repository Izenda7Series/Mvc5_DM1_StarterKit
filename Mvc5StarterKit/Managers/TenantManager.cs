using Mvc5StarterKit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mvc5StarterKit.Managers
{
    public class TenantManager
    {
        #region Methods
        public Tenant GetTenantByName(string name)
        {
            using (var context = ApplicationDbContext.Create())
            {
                var tenant = context.Tenants.Where(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();

                return tenant;
            }
        }

        public IEnumerable<string> GetAllTenants()
        {
            using (var context = ApplicationDbContext.Create())
            {
                var tenantList = new List<string>();

                foreach (var tenant in context.Tenants)
                {
                    if (!tenant.Name.Equals("System", StringComparison.InvariantCultureIgnoreCase))
                        tenantList.Add(tenant.Name);
                }

                return tenantList;
            }
        }

        public async Task<Tenant> SaveTenantAsync(Tenant tenant)
        {
            using (var context = ApplicationDbContext.Create())
            {
                context.Tenants.Add(tenant);
                await context.SaveChangesAsync();

                return tenant;
            }
        } 
        #endregion
    }
}
