
using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using BSOFT.Infrastructure.Repositories;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Infrastructure.Repositories
{
    public class RoleEntitlementRepository : IRoleEntitlementRepository
    {
    private readonly ApplicationDbContext _applicationDbContext;
    public RoleEntitlementRepository(ApplicationDbContext applicationDbContext)
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


    public async Task AddRoleEntitlementsAsync(IEnumerable<RoleEntitlement> roleEntitlements, CancellationToken cancellationToken)
    {
        await _applicationDbContext.RoleEntitlements.AddRangeAsync(roleEntitlements, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRoleEntitlementsAsync(int roleId, List<RoleEntitlement> roleEntitlements, CancellationToken cancellationToken)
    {
        // Remove existing entitlements for the role
        var existingEntitlements = _applicationDbContext.RoleEntitlements.Where(re => re.RoleId == roleId);
        _applicationDbContext.RoleEntitlements.RemoveRange(existingEntitlements);

        // Add updated entitlements
        await _applicationDbContext.RoleEntitlements.AddRangeAsync(roleEntitlements, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
    }

        // New validation methods
    public async Task<bool> ModuleExistsAsync(int moduleId, CancellationToken cancellationToken)
    {
        return await _applicationDbContext.Modules.AnyAsync(m => m.Id == moduleId, cancellationToken);
    }

    public async Task<bool> MenuExistsAsync(int menuId, CancellationToken cancellationToken)
    {
        return await _applicationDbContext.Menus.AnyAsync(m => m.Id == menuId, cancellationToken);
    }

    }
}