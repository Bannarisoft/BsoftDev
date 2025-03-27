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
  //  public async Task<List<Department>>GetAllDepartmentAsync()
    public async Task<(List<Department>,int)> GetAllDepartmentAsync(int PageNumber, int PageSize, string? SearchTerm)
    {
        
           var query = $$"""
             DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
               FROM AppData.Department
              WHERE IsDeleted = 0  
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (ShortName LIKE @Search OR DeptName LIKE @Search)")}};

             SELECT Id
                ,CompanyId
                ,ShortName
                ,DeptName
                ,CreatedIP
                ,IsActive
                ,CreatedBy
                ,CreatedByName
                ,CreatedAt
                ,ModifiedBy
                ,ModifiedByName
                ,ModifiedAt
                ,ModifiedIP
                ,IsDeleted
                FROM AppData.Department WHERE IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (ShortName LIKE @Search OR DeptName LIKE @Search )")}}
            ORDER BY Id DESC              
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            SELECT @TotalCount AS TotalCount ;
            """;

           var parameters = new
                       {
                           Search = $"%{SearchTerm}%",
                           Offset = (PageNumber - 1) * PageSize,
                           PageSize
                       };

               var department = await _dbConnection.QueryMultipleAsync(query, parameters);
             var departmentlist = (await department.ReadAsync<Department>()).ToList();
             int totalCount = (await department.ReadFirstAsync<int>());
            return (departmentlist, totalCount);        
            
        
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
            
           const string query = @"
            SELECT Id,CompanyId,ShortName,DeptName,IsActive FROM  AppData.Department 
            WHERE (DeptName LIKE @SearchDept OR  ShortName LIKE @SearchDept) and IsDeleted = 0
            ORDER BY Id DESC";

              var parameters = new 
              { 
                  SearchDept = $"%{SearchDept ?? string.Empty}%"
              };
           
            var departments = await _dbConnection.QueryAsync<Department>(query, parameters);
            return departments.ToList();       
        }
            public async Task<bool> FKColumnExistValidation(int Id)
          {
              var sql = "SELECT COUNT(1) FROM AppData.Department WHERE Id = @Id AND IsDeleted = 0 AND IsActive = 1";
                var count = await _dbConnection.ExecuteScalarAsync<int>(sql, new { Id = Id });
                return count > 0;
          }

       
    }
}