
using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using BSOFT.Infrastructure.Repositories;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IRoleEntitlement;

namespace BSOFT.Infrastructure.Repositories.RoleEntitlements
{
    public class RoleEntitlementQueryRepository : IRoleEntitlementQueryRepository
    {
    private readonly ApplicationDbContext _applicationDbContext;
    public RoleEntitlementQueryRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<UserRole> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken)
    {
        return await _applicationDbContext.UserRole.FirstOrDefaultAsync(r => r.RoleName == roleName, cancellationToken) ?? new UserRole();
    }

    public async Task<List<RoleEntitlement>> GetRoleEntitlementsByRoleNameAsync(string roleName, CancellationToken cancellationToken)
    {
        return await _applicationDbContext.RoleEntitlements
            .Where(re => re.UserRole.RoleName == roleName)
            .Include(re => re.Module)
            .Include(re => re.Menu)
            .ToListAsync(cancellationToken);
    }
    }
}