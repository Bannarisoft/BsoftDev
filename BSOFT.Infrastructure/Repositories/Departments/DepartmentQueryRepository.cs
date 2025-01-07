using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IDepartment;
using System.Data;
using Dapper;
using Core.Application.Departments.Queries.GetDepartments;

namespace BSOFT.Infrastructure.Repositories.Departments
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
        
        const string query = @"
           SELECT  * FROM AppData.Department";
            return (await _dbConnection.QueryAsync<Department>(query)).ToList();
        
    }

    public async Task<Department> GetByIdAsync(int id)
    {
           const string query = "SELECT * FROM AppData.Department WHERE Id = @Id";
        return await _dbConnection.QueryFirstOrDefaultAsync<Department>(query, new { id });
        }
    
         

  
       public async Task<List<Department>>GetAllDepartmentAutoCompleteAsync()
    {
         const string query = @"
           select CompanyId,ShortName,DeptName,IsActive from  AppData.Department 
            --WHERE DeptName LIKE @SearchDept OR Id LIKE @SearchDept
            ORDER BY DeptName";
            return (await _dbConnection.QueryAsync<Department>(query)).ToList();

        
        
    }

//   public async Task<List<Department>> GetAllDepartmentAutoCompleteSearchAsync()
//         {
//             return await _applicationDbContext.Department.ToListAsync();
//         }

    public async Task<List<Department>>  GetAllDepartmentAutoCompleteSearchAsync(string SearchDept = null)
        {
            if (string.IsNullOrWhiteSpace(SearchDept))
            {
                throw new ArgumentException("DepartmentName cannot be null or empty.", nameof(SearchDept));
            }

           const string query = @"
            select Id,CompanyId,ShortName,DeptName,IsActive from  AppData.Department 
            WHERE DeptName LIKE @SearchDept OR Id LIKE @SearchDept and IsActive =1
            ORDER BY DeptName";
            // Update the object to use SearchPattern instead of Name
          var departments = await _dbConnection.QueryAsync<Department>(query, new { SearchDept = $"%{SearchDept}%" });
            return departments.ToList();
        }

    }
}