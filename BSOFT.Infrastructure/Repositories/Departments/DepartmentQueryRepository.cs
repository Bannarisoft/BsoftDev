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
        
        const string query = @"SELECT  * FROM AppData.Department WHERE IsDeleted = 0";
            return (await _dbConnection.QueryAsync<Department>(query)).ToList();
        
    }

    public async Task<Department?> GetByIdAsync(int id)
    {
        const string query = @"SELECT * FROM AppData.Department WHERE Id = @Id AND IsDeleted = 0";
        var departments = await _dbConnection.QueryAsync<Department>(query, new { Id = id });

    var department = departments.FirstOrDefault();

    if (department == null)
    {
       return null;
    }

    return department; // Returns null if not found
        //  const string query = @"SELECT * FROM AppData.Department WHERE Id = @Id AND IsDeleted = 0";
        //    // var department = await _dbConnection.QueryFirstOrDefaultAsync<Department>(query, new { id });           
        //     //  if (department == null)
        //     // {
        //     //   ///  throw new KeyNotFoundException($"Department with ID {id} not found.");
        //     //   return null;
        //     // }
        //       var department = await _dbConnection.QueryAsync<Department>(query, new { id });
        //      return department ?? new List<Department>();
        }                  
    



    public async Task<List<Department>>  GetAllDepartmentAutoCompleteSearchAsync(string SearchDept)
        {
            if (string.IsNullOrWhiteSpace(SearchDept))
            {
                throw new ArgumentException("DepartmentName cannot be null or empty.", nameof(SearchDept));
            }

            
           const string query = @"
            select Id,CompanyId,ShortName,DeptName,IsActive from  AppData.Department 
            WHERE (DeptName LIKE @SearchDept OR Id LIKE @SearchDept) and IsDeleted = 0
            ORDER BY DeptName";
           
          var departments = await _dbConnection.QueryAsync<Department>(query, new { SearchDept = $"%{SearchDept}%" });
            return departments.ToList();
        }

       
    }
}