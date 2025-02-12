using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Data;
using Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUserRole;
using System.Data;
using Dapper;
using Core.Application.UserRole.Queries.GetRole;

namespace UserManagement.Infrastructure.Repositories.UserRoles
{
    public class UserRoleQueryRepository :IUserRoleQueryRepository
    {
        
         private readonly IDbConnection _dbConnection;  

    public  UserRoleQueryRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }


            public async Task<(List<UserRole>, int)> GetAllRoleAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var query = $$"""
                DECLARE @TotalCount INT;
                SELECT @TotalCount = COUNT(*) 
                FROM AppSecurity.UserRole 
                WHERE IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (RoleName LIKE @Search OR Description LIKE @Search)")}};
                
                SELECT 
                    Id, 
                    RoleName, 
                    Description, 
                    CompanyId, 
                    IsActive, 
                    CreatedBy, 
                    CreatedAt, 
                    CreatedByName, 
                    CreatedIP, 
                    ModifiedBy, 
                    ModifiedAt, 
                    ModifiedByName, 
                    ModifiedIP, 
                    IsDeleted
                FROM AppSecurity.UserRole 
                WHERE 
                    IsDeleted = 0
                    {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (RoleName LIKE @Search OR Description LIKE @Search)")}}
                ORDER BY Id DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                
                SELECT @TotalCount AS TotalCount;
            """;

            var parameters = new
            {
                Search = $"%{SearchTerm}%",
                Offset = (PageNumber - 1) * PageSize,
                PageSize
            };

            var roles = await _dbConnection.QueryMultipleAsync(query, parameters);
            var roleList = (await roles.ReadAsync<UserRole>()).ToList();
            int totalCount = (await roles.ReadFirstAsync<int>());

            return (roleList, totalCount);
        }


            public async Task<UserRole> GetByIdAsync(int id)
            {
                //return await _applicationDbContext.UserRole.AsNoTracking().FirstOrDefaultAsync(b=>b.Id==id);     

                const string query = "SELECT Id,RoleName,Description,CompanyId,IsActive FROM  AppSecurity.UserRole WHERE Id = @Id AND IsDeleted=0 ORDER BY Id DESC";
                return await _dbConnection.QueryFirstOrDefaultAsync<UserRole>(query, new { id });
            
            }   
                public async Task<List<UserRole>> GetRolesAsync(string searchTerm = null)
            {
                const string query = @"
                    SELECT Id, RoleName, Description, CompanyId, IsActive, CreatedBy, CreatedAt, CreatedByName, CreatedIP, 
                        ModifiedBy, ModifiedAt, ModifiedByName, ModifiedIP, IsDeleted
                    FROM AppSecurity.UserRole 
                    WHERE (RoleName LIKE @searchTerm OR CAST(Id AS NVARCHAR) LIKE @searchTerm) AND IsDeleted = 0
                    ORDER BY Id DESC";
                
                var parameters = new
                {
                    searchTerm = $"%{searchTerm ?? string.Empty}%"
                };

                var userRoles = await _dbConnection.QueryAsync<UserRole>(query, parameters);
                return userRoles.ToList();
            }

   
    }
}