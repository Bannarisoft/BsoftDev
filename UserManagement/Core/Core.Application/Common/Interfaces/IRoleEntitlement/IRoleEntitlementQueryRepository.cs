using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IRoleEntitlement
{
    public interface IRoleEntitlementQueryRepository
    {
        Task<(Core.Domain.Entities.UserRole,List<Core.Domain.Entities.RoleModule>, List<Domain.Entities.Menu>,List<RoleMenu>)> GetByIdAsync(int Id);
        Task<Core.Domain.Entities.UserRole> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken);
        Task<List<RoleEntitlement>> GetRoleEntitlementsByRoleNameAsync(string roleName, CancellationToken cancellationToken);                      
        Task<List<RoleEntitlement>> GetExistingRoleEntitlementsAsync(List<int> userRoleIds,  List<int> menuIds, CancellationToken cancellationToken);
    }
}