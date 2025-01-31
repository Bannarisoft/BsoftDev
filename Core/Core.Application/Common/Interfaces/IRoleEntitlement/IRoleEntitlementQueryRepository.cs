using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IRoleEntitlement
{
    public interface IRoleEntitlementQueryRepository
    {
        Task<RoleEntitlement> GetByIdAsync(int Id);
        Task<Core.Domain.Entities.UserRole> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken);
        Task<List<RoleEntitlement>> GetRoleEntitlementsByRoleNameAsync(string roleName, CancellationToken cancellationToken);                      
        Task<List<RoleEntitlement>> GetExistingRoleEntitlementsAsync(List<int> userRoleIds, List<int> moduleIds, List<int> menuIds, CancellationToken cancellationToken);
    }
}