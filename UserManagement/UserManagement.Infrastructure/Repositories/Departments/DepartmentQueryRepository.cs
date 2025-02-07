using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Data;
using Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IDepartment;
using System.Data;
using Dapper;
using Core.Application.Departments.Queries.GetDepartments;

namespace UserManagement.Infrastructure.Repositories.Departments
{
    public class DepartmentQueryRepository :IDepartmentQueryRepository
    { 
    private readonly IDbConnection _dbConnection; 

    public  DepartmentQueryRepository(IDbConnection dbConnection)
    {
         _dbConnection = dbConnection;
    }
    public async Task<List<Department>>GetAllDepartmentAsync()
    {
        
        const string query = @"SELECT  * FROM AppData.Department WHERE IsDeleted = 0 ORDER BY Id DESC ";
            return (await _dbConnection.QueryAsync<Department>(query)).ToList();
        
    }

    public async Task<Department?> GetByIdAsync(int id)
    {
        const string query = @"SELECT * FROM AppData.Department WHERE Id = @Id AND IsDeleted = 0 ORDER BY Id DESC ";
        var departments = await _dbConnection.QueryAsync<Department>(query, new { Id = id });

    var department = departments.FirstOrDefault();

    if (department == null)
    {
       return null;
    }

    return department; // Returns null if not found
      
        }       
        public async Task<List<Department>>  GetAllDepartmentAutoCompleteSearchAsync(string SearchDept)
        {
            if (string.IsNullOrWhiteSpace(SearchDept))
            {
                throw new ArgumentException("DepartmentName cannot be null or empty.", nameof(SearchDept));
            }

            
           const string query = @"
            SELECT Id,CompanyId,ShortName,DeptName,IsActive FROM  AppData.Department 
            WHERE (DeptName LIKE @SearchDept OR Id LIKE @SearchDept) and IsDeleted = 0
            ORDER BY Id DESC";
           
          var departments = await _dbConnection.QueryAsync<Department>(query, new { SearchDept = $"%{SearchDept}%" });
            return departments.ToList();
        }

       
    }
}