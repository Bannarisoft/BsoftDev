using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUserGroup;
using Dapper;

namespace UserManagement.Infrastructure.Repositories.UserGroup
{
    public class UserGroupQueryRepository :IUserGroupQueryRepository
    {
         private readonly IDbConnection _dbConnection; 
        public  UserGroupQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<List<Core.Domain.Entities.UserGroup>> GetUserGroups(string searchTerm = null)
        {
            const string query = @"
            SELECT Id, GroupCode, GroupName,  IsActive, CreatedBy, CreatedAt, CreatedByName, CreatedIP, 
                ModifiedBy, ModifiedAt, ModifiedByName, ModifiedIP, IsDeleted
            FROM AppSecurity.UserGroup
            WHERE (GroupName LIKE @searchTerm OR CAST(Id AS NVARCHAR) LIKE @searchTerm) AND IsDeleted = 0
            ORDER BY Id DESC";

            var parameters = new
            {
            searchTerm = $"%{searchTerm ?? string.Empty}%"
            };

            var userRoles = await _dbConnection.QueryAsync<Core.Domain.Entities.UserGroup>(query, parameters);
            return userRoles.ToList();
        }
    }
}