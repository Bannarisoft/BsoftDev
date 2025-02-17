
using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Data;
using UserManagement.Infrastructure.Repositories;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IRoleEntitlement;
using System.Data;
using Dapper;
using Serilog;

namespace UserManagement.Infrastructure.Repositories.RoleEntitlements
{
        public class RoleEntitlementQueryRepository : IRoleEntitlementQueryRepository
        {
        private readonly IDbConnection _dbConnection;

        public RoleEntitlementQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;   

        }
        public async Task<(Core.Domain.Entities.UserRole,List<Core.Domain.Entities.RoleModule>, List<Menu>,List<RoleMenu>)> GetByIdAsync(int roleEntitlementId)
        {

            var  query = @"
                SELECT  Id FROM [AppSecurity].[UserRole] WHERE Id = @RoleEntitlementId

                SELECT Id,ModuleId FROM [AppSecurity].[RoleModule] WHERE RoleId=@RoleEntitlementId

                SELECT m.Id,m.MenuName,m.ModuleId,m.ParentId FROM [AppData].[Menus] m
                Inner join [AppSecurity].[RoleMenu] rmenu ON rmenu.MenuId=m.Id
                Inner join [AppSecurity].[RoleModule] rm ON rm.Id=rmenu.RoleModuleId
                 WHERE   rm.RoleId = @RoleEntitlementId
                
                SELECT rmenu.Id,rmenu.RoleModuleId,rmenu.MenuId,rmenu.CanView,rmenu.CanAdd,rmenu.CanUpdate,rmenu.CanDelete,rmenu.CanApprove,rmenu.CanExport,rmenu.CanView  FROM   [AppSecurity].[RoleModule] rm
	             Inner join [AppSecurity].[RoleMenu] rmenu ON rm.Id=rmenu.RoleModuleId
                 WHERE rm.RoleId = @RoleEntitlementId
                
                 ";

                    using var multi = await _dbConnection.QueryMultipleAsync(query, new { RoleEntitlementId = roleEntitlementId });

            
           var role = await multi.ReadFirstOrDefaultAsync<UserRole>();

             // Read Modules
             var modules = (await multi.ReadAsync<Core.Domain.Entities.RoleModule>()).ToList();

             // Read Parent Menus (Menus with ParentId = 0)
            //  var parentIds = (await multi.ReadAsync<Menu>()).ToList();

             // Read All Child Menus
             var allMenus = (await multi.ReadAsync<Menu>()).ToList();

             // Read RoleMenu Permissions
             var roleMenus = (await multi.ReadAsync<RoleMenu>()).ToList();
           
            var menuHierarchy = GetChildMenus(allMenus.ToList());
          
           

            return (role, modules, menuHierarchy,roleMenus);
        }
        
        private List<Menu> GetChildMenus( List<Menu> allMenus)
        {
            var menuDict = allMenus.ToDictionary(m => m.Id);
            List<Menu> rootMenus = new();

              foreach (var menu in allMenus)
              {
                  if (menu.ParentId != 0 && menuDict.ContainsKey(menu.ParentId))
                  {
                      menuDict[menu.ParentId].ChildMenus.Add(menu);
                  }
                  else
                  {
                      rootMenus.Add(menu);
                  }
              }
              return rootMenus;
            // var childMenus = allMenus.Where(m => m.ParentId == menuid && m.ModuleId == moduleId).ToList();

            // foreach (var child in childMenus)
            // {
            //     // Assign RoleMenu Permissions
            //     var permissions = roleMenus.Where(rm => rm.MenuId == child.Id && rm.RoleModuleId == RoleModuleId).ToList();
                

            //     if (permissions.Any())
            //     {
            //         child.RoleMenus = permissions;
                    
                  
            //     }

            //     // Recursively Assign Nested Child Menus (Grandchildren)
                
            // }

            // return childMenus;
        }


        public async Task<List<RoleEntitlement>> GetExistingRoleEntitlementsAsync(List<int> userRoleIds,  List<int> menuIds, CancellationToken cancellationToken)
        {
            if (!userRoleIds.Any() || !menuIds.Any())
            {
                return new List<RoleEntitlement>(); // Return empty list if any input list is empty
            }
           const string query = @"
                SELECT UserRoleId, ModuleId, MenuId 
                FROM AppSecurity.RoleEntitlements
                WHERE UserRoleId IN @UserRoleIds 
                AND ModuleId IN @ModuleIds 
                AND MenuId IN @MenuIds";

            var parameters = new
            {
                UserRoleIds = userRoleIds,
                // ModuleIds = moduleIds,
                MenuIds = menuIds
            };

            var existingEntitlements = await _dbConnection.QueryAsync<RoleEntitlement>(query, parameters);
            return existingEntitlements.ToList();
            // return await _applicationDbContext.RoleEntitlements
            // .Where(re => userRoleIds.Contains(re.UserRoleId) && moduleIds.Contains(re.ModuleId) && menuIds.Contains(re.MenuId))
            // .ToListAsync(cancellationToken);

        }

        public async Task<UserRole> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken)
        {
        // return await _applicationDbContext.UserRole.FirstOrDefaultAsync(r => r.RoleName == roleName, cancellationToken) ?? new UserRole();
            const string query = @"
        SELECT Id, RoleName, IsActive, CreatedBy, CreatedAt, CreatedByName, CreatedIP, 
               ModifiedBy, ModifiedAt, ModifiedByName, ModifiedIP
        FROM AppSecurity.UserRole 
        WHERE RoleName = @RoleName";

        var userRole = await _dbConnection.QueryFirstOrDefaultAsync<UserRole>(query, new { RoleName = roleName });

        return userRole ?? new UserRole();
        }

        public async Task<List<RoleEntitlement>> GetRoleEntitlementsByRoleNameAsync(string roleName, CancellationToken cancellationToken)
        {
        // return await _applicationDbContext.RoleEntitlements
        //     .Where(re => re.UserRole.RoleName == roleName)
        //     .Include(re => re.Module)
        //     .Include(re => re.Menu)
        //     .ToListAsync(cancellationToken);
        const string query = @"
        SELECT re.Id, re.IsActive, re.CreatedBy, re.CreatedAt, re.CreatedByName, re.CreatedIP, 
               re.ModifiedBy, re.ModifiedAt, re.ModifiedByName, re.ModifiedIP, 
               ur.Id AS UserRoleId, ur.RoleName, 
               re.ModuleId, m.ModuleName, 
               re.MenuId, mn.MenuName
        FROM AppSecurity.RoleEntitlements re
        INNER JOIN AppSecurity.UserRole ur ON re.UserRoleId = ur.Id
        LEFT JOIN AppData.Menus mn ON re.MenuId = mn.Id
        LEFT JOIN AppData.Modules m ON mn.ModuleId = m.Id
        WHERE ur.RoleName = @RoleName AND re.IsDeleted = 0";

    var roleEntitlements = await _dbConnection.QueryAsync<RoleEntitlement, UserRole, Modules, Menu, RoleEntitlement>(
        query,
        (re, ur, m, mn) =>
        {
            re.UserRole = ur;
            mn.Module = m;
            re.Menu = mn;
            return re;
        },
        new { RoleName = roleName },
        splitOn: "UserRoleId,ModuleId,MenuId"
    );

    return roleEntitlements.ToList();
    }
    }
}