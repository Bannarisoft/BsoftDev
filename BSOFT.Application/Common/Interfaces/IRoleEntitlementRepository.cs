using BSOFT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Common.Interfaces
{
    public interface IRoleEntitlementRepository
    {
    // Task<RoleEntitlement> GetAsync(int id);
    // Task CreateAsync(RoleEntitlement roleEntitlement);
    // Task UpdateAsync(RoleEntitlement roleEntitlement);
    // Task DeleteAsync(int id);
    Task AddRoleEntitlementsAsync(IEnumerable<RoleEntitlement> roleEntitlements);
    Task<List<Modules>> GetModulesWithMenusAsync();
    }

}