using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces
{
    public interface IRoleEntitlementRepository
    {
        Task<Core.Domain.Entities.UserRole> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken);
        Task AddRoleEntitlementsAsync(IEnumerable<RoleEntitlement> roleEntitlements, CancellationToken cancellationToken);
        Task<List<RoleEntitlement>> GetRoleEntitlementsByRoleNameAsync(string roleName, CancellationToken cancellationToken);
        Task UpdateRoleEntitlementsAsync(int roleId, List<RoleEntitlement> roleEntitlements, CancellationToken cancellationToken);

               // Add these new method definitions
        Task<bool> ModuleExistsAsync(int moduleId, CancellationToken cancellationToken);
        Task<bool> MenuExistsAsync(int menuId, CancellationToken cancellationToken);
    }
}