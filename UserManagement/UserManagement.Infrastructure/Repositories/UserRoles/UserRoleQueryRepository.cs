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
    public async Task<List<UserRole>>GetAllRoleAsync()
    {
        
        //return await _applicationDbContext.UserRole.ToListAsync();
         const string query = @"SELECT  * FROM AppSecurity.UserRole";
        return (await _dbConnection.QueryAsync<UserRole>(query)).ToList();
    }

    public async Task<UserRole> GetByIdAsync(int id)
    {
        //return await _applicationDbContext.UserRole.AsNoTracking().FirstOrDefaultAsync(b=>b.Id==id);     

          const string query = "SELECT Id,RoleName,Description,CompanyId,IsActive from  AppSecurity.UserRole WHERE Id = @Id";
          return await _dbConnection.QueryFirstOrDefaultAsync<UserRole>(query, new { id });
    
    }   
    public async Task<List<UserRole>> GetRolesAsync(string searchTerm)
    {
        /* return await _applicationDbContext.UserRole
        .Where(r => EF.Functions.Like(r.RoleName, $"%{searchTerm}%")) // Case-insensitive search
        .Select(r => new UserRole
        {
            Id = r.Id,      
            RoleName = r.RoleName    
        })
        .ToListAsync(); */
          if (string.IsNullOrWhiteSpace(searchTerm))
            {
                throw new ArgumentException("UserRole cannot be null or empty.", nameof(searchTerm));
            }

            const string query = @"
                 SELECT  Id,RoleName,Description,CompanyId,IsActive from  AppSecurity.UserRole 
            WHERE RoleName LIKE @searchTerm OR Id LIKE @searchTerm and IsActive =1
            ORDER BY RoleName";
                
            // Update the object to use SearchPattern instead of Name
            var result = await _dbConnection.QueryAsync<UserRole>(query, new { SearchTerm = $"%{searchTerm}%" });
            return result.ToList();
    }
    }
}