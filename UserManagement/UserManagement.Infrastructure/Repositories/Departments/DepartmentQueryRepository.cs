using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Data;
using Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IDepartment;
using System.Data;
using Dapper;
using Core.Application.Departments.Queries.GetDepartments;
using Core.Application.Common.Interfaces;
using Core.Application.Departments.Queries.GetDepartmentByGroupWithControl;

namespace UserManagement.Infrastructure.Repositories.Departments
{
  public class DepartmentQueryRepository : IDepartmentQueryRepository
  {
    private readonly IDbConnection _dbConnection;
    private readonly IIPAddressService _ipAddressService;

    public DepartmentQueryRepository(IDbConnection dbConnection, IIPAddressService ipAddressService)
    {
      _dbConnection = dbConnection;
      _ipAddressService = ipAddressService;
    }
   

    public async Task<(List<DepartmentDto>, int)> GetAllDepartmentAsync(int PageNumber, int PageSize, string? SearchTerm)
    {
      
      var query = $$"""
            DECLARE @TotalCount INT;

            SELECT @TotalCount = COUNT(*)
            FROM AppData.Department D
            INNER JOIN AppData.DepartmentGroup DG ON DG.Id = D.DepartmentGroupId
            WHERE D.IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (D.ShortName LIKE @Search OR D.DeptName LIKE @Search OR DG.DepartmentGroupName LIKE @Search)")}};

            SELECT 
                D.Id,
                D.CompanyId,
                D.ShortName,
                D.DeptName,
                D.DepartmentGroupId,
                DG.DepartmentGroupName AS DepartmentGroupName, 
                D.CreatedIP,
                D.IsActive,
                D.CreatedBy,
                D.CreatedByName,
                D.CreatedAt,
                D.ModifiedBy,
                D.ModifiedByName,
                D.ModifiedAt,
                D.ModifiedIP,
                D.IsDeleted
            FROM AppData.Department D
            INNER JOIN AppData.DepartmentGroup DG ON DG.Id = D.DepartmentGroupId
            WHERE D.IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (D.ShortName LIKE @Search OR D.DeptName LIKE @Search OR DG.DepartmentGroupName LIKE @Search)")}}
            ORDER BY D.Id DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

            SELECT @TotalCount AS TotalCount;
        """;

      var parameters = new
      {
        Search = $"%{SearchTerm}%",
        Offset = (PageNumber - 1) * PageSize,
        PageSize
      };

      var result = await _dbConnection.QueryMultipleAsync(query, parameters);
      var departmentList = (await result.ReadAsync<DepartmentDto>()).ToList(); // map to DTO if needed
      int totalCount = await result.ReadFirstAsync<int>();

      return (departmentList, totalCount);
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

    public async Task<List<DepartmentWithGroupDto>> GetDepartmentsByDepartmentGroupIdAsync(string departmentGroupName)
    {
      const string query = @"
            SELECT 
                D.Id,
                D.CompanyId,
                D.ShortName,
                D.DeptName,
                D.DepartmentGroupId,
                DG.DepartmentGroupName,
                D.IsActive
            FROM AppData.Department D           
            INNER JOIN AppData.DepartmentGroup DG ON DG.Id = D.DepartmentGroupId
            WHERE DG.DepartmentGroupName = @DepartmentGroupName AND D.IsDeleted = 0 AND D.IsActive = 1";

      var departments = await _dbConnection.QueryAsync<DepartmentWithGroupDto>(query, new { DepartmentGroupName = departmentGroupName });

      return departments.ToList();
    }


    public async Task<List<Department>> GetAllDepartmentAutoCompleteSearchAsync(string SearchDept)
    {
      var CompanyId = _ipAddressService.GetCompanyId();
      var userId = _ipAddressService.GetUserId();
      const string query = @"
            SELECT D.Id,D.CompanyId,D.ShortName,D.DeptName,D.DepartmentGroupId ,DG.DepartmentGroupName,D.IsActive FROM  AppData.Department D
            INNER JOIN [AppSecurity].[UserDepartment] UD ON UD.DepartmentId=D.Id AND UD.IsActive=1
            INNER JOIN [AppData].[DepartmentGroup] DG ON DG.Id=D.DepartmentGroupId
            WHERE (D.DeptName LIKE @SearchDept OR  D.ShortName LIKE @SearchDept) and D.IsDeleted = 0 AND D.CompanyId=@CompanyId AND UD.UserId=@UserId
            ORDER BY D.Id DESC";

      var parameters = new
      {
        SearchDept = $"%{SearchDept ?? string.Empty}%",
        CompanyId = CompanyId,
        UserId = userId
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
    public async Task<List<Department>> GetDepartment_SuperAdmin(string SearchDept)
    {
      var CompanyId = _ipAddressService.GetCompanyId();
      const string query = @"
            SELECT D.Id,D.CompanyId,D.ShortName,D.DeptName,D.IsActive FROM  AppData.Department D
            WHERE (D.DeptName LIKE @SearchDept OR  D.ShortName LIKE @SearchDept) and D.IsDeleted = 0 AND D.CompanyId=@CompanyId 
            ORDER BY D.Id DESC";

      var parameters = new
      {
        SearchDept = $"%{SearchDept ?? string.Empty}%",
        CompanyId = CompanyId
      };

      var departments = await _dbConnection.QueryAsync<Department>(query, parameters);
      return departments.ToList();
    }

        public async Task<List<DepartmentWithControlByGroupDto>> GetDepartmentsByDepartmentGroupWithControl(string departmentGroupName)
        {
          var userId = _ipAddressService.GetUserId();
                   const string query = @"
                 SELECT 
                     D.Id,
                     D.ShortName,
                     D.DeptName,
                     D.DepartmentGroupId,
                     DG.DepartmentGroupName
                 FROM AppData.Department D           
                 INNER JOIN AppData.DepartmentGroup DG ON DG.Id = D.DepartmentGroupId
                 INNER JOIN [AppSecurity].[UserDepartment] UD ON UD.DepartmentId = D.Id
                 WHERE DG.DepartmentGroupName = @DepartmentGroupName AND D.IsDeleted = 0 AND D.IsActive = 1 AND UD.UserId =@UserId AND UD.IsActive=1";

           var departments = await _dbConnection.QueryAsync<DepartmentWithControlByGroupDto>(query, new { DepartmentGroupName = departmentGroupName,UserId = userId});

           return departments.ToList();
        }
    }
}