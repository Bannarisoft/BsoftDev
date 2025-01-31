
using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using BSOFT.Infrastructure.Repositories;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IRoleEntitlement;
using System.Data;
using Dapper;
using Serilog;

namespace BSOFT.Infrastructure.Repositories.RoleEntitlements
{
        public class RoleEntitlementQueryRepository : IRoleEntitlementQueryRepository
        {
        private readonly IDbConnection _dbConnection;

        public RoleEntitlementQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;

        }
        public async Task<RoleEntitlement> GetByIdAsync(int roleEntitlementId)
        {
            if (roleEntitlementId <= 0)
            {
                Log.Error("Invalid RoleEntitlementId: {RoleEntitlementId}", roleEntitlementId);
                throw new ArgumentException("RoleEntitlementId must be greater than zero.", nameof(roleEntitlementId));
            }

            const string query = @"
                SELECT re.Id, re.IsActive,m.Id as ModuleId, m.ModuleName,mn.Id as MenuId,mn.MenuName,ur.RoleName,CanAdd,CanDelete,CanExport,CanUpdate,CanView,CanApprove
            FROM AppSecurity.RoleEntitlements re WITH (NOLOCK)
			INNER JOIN AppSecurity.UserRole ur on re.UserRoleId=ur.Id
			INNER JOIN AppData.Modules m on re.ModuleId=m.Id
			INNER JOIN AppData.Menus mn on re.MenuId=mn.Id
            WHERE re.Id = @RoleEntitlementId  AND re.IsActive = 1";
                  
            var roleEntitlement = await _dbConnection.QuerySingleOrDefaultAsync<RoleEntitlement>(query, new { RoleEntitlementId = roleEntitlementId });

            if (roleEntitlement == null)
            {
                Log.Warning("RoleEntitlement with ID {RoleEntitlementId} not found.", roleEntitlementId);
                throw new KeyNotFoundException($"RoleEntitlement with ID {roleEntitlementId} not found.");
            }

            Log.Information("Successfully fetched RoleEntitlement with ID {RoleEntitlementId}", roleEntitlementId);
            return roleEntitlement;
        }

        public async Task<List<RoleEntitlement>> GetExistingRoleEntitlementsAsync(List<int> userRoleIds, List<int> moduleIds, List<int> menuIds, CancellationToken cancellationToken)
        {
            if (!userRoleIds.Any() || !moduleIds.Any() || !menuIds.Any())
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
                ModuleIds = moduleIds,
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
        FROM AppSecurity.UserRole WITH (NOLOCK)
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
        LEFT JOIN AppData.Modules m ON re.ModuleId = m.Id
        LEFT JOIN AppData.Menus mn ON re.MenuId = mn.Id
        WHERE ur.RoleName = @RoleName";

    var roleEntitlements = await _dbConnection.QueryAsync<RoleEntitlement, UserRole, Modules, Menu, RoleEntitlement>(
        query,
        (re, ur, m, mn) =>
        {
            re.UserRole = ur;
            re.Module = m;
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