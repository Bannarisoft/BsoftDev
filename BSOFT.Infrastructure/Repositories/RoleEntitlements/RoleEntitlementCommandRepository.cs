
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
    public class RoleEntitlementCommandRepository : IRoleEntitlementCommandRepository
    {
    private readonly ApplicationDbContext _applicationDbContext;
    public RoleEntitlementCommandRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

  
    public async Task AddRoleEntitlementsAsync(IEnumerable<RoleEntitlement> roleEntitlements, CancellationToken cancellationToken)
    {
        await _applicationDbContext.RoleEntitlements.AddRangeAsync(roleEntitlements, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRoleEntitlementsAsync(int roleId, List<RoleEntitlement> roleEntitlements, CancellationToken cancellationToken)
    {
        // Remove existing entitlements for the role
        var existingEntitlements = _applicationDbContext.RoleEntitlements.Where(re => re.UserRoleId == roleId);
        _applicationDbContext.RoleEntitlements.RemoveRange(existingEntitlements);

        // Add updated entitlements
        await _applicationDbContext.RoleEntitlements.AddRangeAsync(roleEntitlements, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task<int> DeleteAsync(int id, RoleEntitlement roleEntitlement)
    {
        var RoleEntitlementToDelete = await _applicationDbContext.RoleEntitlements.FirstOrDefaultAsync(u => u.Id == id);
            if (RoleEntitlementToDelete != null)
            {
                RoleEntitlementToDelete.IsActive = roleEntitlement.IsActive;
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
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