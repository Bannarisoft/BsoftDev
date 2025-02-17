
using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Data;
using UserManagement.Infrastructure.Repositories;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IRoleEntitlement;

namespace UserManagement.Infrastructure.Repositories.RoleEntitlements
{
    public class RoleEntitlementCommandRepository : IRoleEntitlementCommandRepository
    {
    private readonly ApplicationDbContext _applicationDbContext;
    public RoleEntitlementCommandRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

  
    public async Task<IList<RoleModule>> AddRoleEntitlementsAsync(IList<RoleModule> roleEntitlements, CancellationToken cancellationToken)
    {
        await _applicationDbContext.RoleModules.AddRangeAsync(roleEntitlements, cancellationToken);
                      await _applicationDbContext.SaveChangesAsync();
                      return roleEntitlements;
        
    }

    public async Task<bool> UpdateRoleEntitlementsAsync(int roleId, IList<RoleModule> roleEntitlements, CancellationToken cancellationToken)
    {
    
         var existingRoleModules  = await _applicationDbContext.RoleModules
                                    .Where(re => re.RoleId == roleId)
                                    .ToListAsync(cancellationToken);

         
          if (existingRoleModules.Any())
          {
              var roleModuleIds = existingRoleModules.Select(rm => rm.Id).ToList();

              var roleMenusToRemove = await _applicationDbContext.RoleMenus
                  .Where(rm => roleModuleIds.Contains(rm.RoleModuleId))
                  .ToListAsync(cancellationToken);

              if (roleMenusToRemove.Any())
              {
                  _applicationDbContext.RoleMenus.RemoveRange(roleMenusToRemove);
              }


             _applicationDbContext.RoleModules.RemoveRange(existingRoleModules);
        }

                
         await _applicationDbContext.RoleModules.AddRangeAsync(roleEntitlements, cancellationToken);
                 

    
         return await _applicationDbContext.SaveChangesAsync(cancellationToken)>0;

    
    }
    public async Task<int> DeleteAsync(int id, RoleEntitlement roleEntitlement)
    {
        var RoleEntitlementToDelete = await _applicationDbContext.RoleEntitlements.FirstOrDefaultAsync(u => u.Id == id);
            if (RoleEntitlementToDelete != null)
            {
                RoleEntitlementToDelete.IsDeleted = roleEntitlement.IsDeleted;
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

//  public async Task<UserRole> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken)
//     {
//         return await _applicationDbContext.UserRole.FirstOrDefaultAsync(r => r.RoleName == roleName, cancellationToken) ?? new UserRole();
//     }

//     public async Task<List<RoleEntitlement>> GetRoleEntitlementsByRoleNameAsync(string roleName, CancellationToken cancellationToken)
//     {
//         return await _applicationDbContext.RoleEntitlements
//             .Where(re => re.UserRole.RoleName == roleName)
//             .Include(re => re.Module)
//             .Include(re => re.Menu)
//             .ToListAsync(cancellationToken);
//     }

//         public async Task<List<RoleEntitlement>> GetExistingRoleEntitlementsAsync(List<int> userRoleIds, List<int> moduleIds, List<int> menuIds, CancellationToken cancellationToken)
//         {
//             return await _applicationDbContext.RoleEntitlements
//             .Where(re => userRoleIds.Contains(re.UserRoleId) && moduleIds.Contains(re.ModuleId) && menuIds.Contains(re.MenuId))
//             .ToListAsync(cancellationToken);
//         }
    }
}