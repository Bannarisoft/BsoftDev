using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IActivityMaster;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.ActivityMaster
{
    public class ActivityMasterQueryRepository  : IActivityMasterQueryRepository
    {
           private readonly IDbConnection _dbConnection;

            public ActivityMasterQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;

        }

        public async Task<(List<Core.Domain.Entities.ActivityMaster>, int)> GetAllActivityMasterAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
                    var query = $$"""
                DECLARE @TotalCount INT;
                SELECT @TotalCount = COUNT(*) 
                FROM [Maintenance].[ActivityMaster] A
                INNER JOIN [Bannari].[AppData].[Department] B ON A.DepartmentId = B.Id
                INNER JOIN [Maintenance].[MachineGroup] C ON A.MachineGroupId = C.Id
                INNER JOIN [Maintenance].[MiscMaster] D ON A.ActivityType = D.Id
                WHERE A.IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (A.ActivityName LIKE @Search OR A.Description LIKE @Search)")}}; 

                SELECT A.Id, A.ActivityName, A.Description, A.DepartmentId, B.DeptName AS Department, 
                    A.MachineGroupId, C.GroupName AS MachineGroup, 
                    A.EstimatedDuration, A.ActivityType, D.Code AS ActivityTypeDescription, 
                    A.IsActive, A.IsDeleted, 
                    A.CreatedBy, A.CreatedDate, A.CreatedByName, A.CreatedIP, 
                    A.ModifiedBy, A.ModifiedDate, A.ModifiedByName, A.ModifiedIP
                FROM [Maintenance].[ActivityMaster] A  
                INNER JOIN [Bannari].[AppData].[Department] B ON A.DepartmentId = B.Id
                INNER JOIN [Maintenance].[MachineGroup] C ON A.MachineGroupId = C.Id
                INNER JOIN [Maintenance].[MiscMaster] D ON A.ActivityType = D.Id
                WHERE A.IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (A.ActivityName LIKE @Search OR A.Description LIKE @Search)")}}
                ORDER BY A.Id DESC 
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

            // Read the data for ActivityMaster and convert to list
            var activityMasterList = (await result.ReadAsync<Core.Domain.Entities.ActivityMaster>()).ToList();

            // Read the total count
            int totalCount = await result.ReadFirstAsync<int>();

            return (activityMasterList, totalCount);
        }

      public async Task<Core.Domain.Entities.ActivityMaster> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT 
                    A.Id, A.ActivityName,  A.Description,  A.DepartmentId, B.DeptName AS Department, A.MachineGroupId, C.GroupName AS MachineGroup, 
                    A.EstimatedDuration,  A.ActivityType,  D.Code AS ActivityTypeDescription,A.IsActive, A.IsDeleted,A.CreatedBy, A.CreatedDate, 
                    A.CreatedByName,  A.CreatedIP,  A.ModifiedBy, A.ModifiedDate,  A.ModifiedByName, A.ModifiedIP  FROM Maintenance.ActivityMaster A  
                INNER JOIN Bannari.AppData.Department B ON A.DepartmentId = B.Id
                INNER JOIN Maintenance.MachineGroup C ON A.MachineGroupId = C.Id
                INNER JOIN Maintenance.MiscMaster D ON A.ActivityType = D.Id         
                WHERE A.Id = @id AND A.IsDeleted = 0";

            return await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.ActivityMaster>(query, new { id });
        } 

          public async Task<List<Core.Domain.Entities.ActivityMaster>> GetActivityMasterAutoComplete(string searchPattern)
               {
                   const string query = @"
                       SELECT Id, ActivityName  
                       FROM Maintenance.ActivityMaster
                       WHERE IsDeleted = 0 AND ActivityName LIKE @SearchPattern";

                   var parameters = new 
                   { 
                       SearchPattern = $"%{searchPattern ?? string.Empty}%"
                   };
               var machineGroups = await _dbConnection.QueryAsync<Core.Domain.Entities.ActivityMaster>(query, parameters);
                   return machineGroups.ToList();
               } 
               
               public async Task<bool> GetByActivityNameAsync(string activityname)
                {
                    var query = """
                    SELECT COUNT(1) FROM Maintenance.ActivityMaster
                    WHERE ActivityName = @activityname AND IsDeleted = 0 
                    """;

                    var result = await _dbConnection.ExecuteScalarAsync<int>(query, new { ActivityName = activityname });

                    return result > 0;
                }





        
    }
}