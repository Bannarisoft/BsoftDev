using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IMachineGroup;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.MachineGroup
{
    public class MachineGroupQueryRepository : IMachineGroupQueryRepository
    {
       
       private readonly IDbConnection _dbConnection;
       
       public MachineGroupQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;

        }
       
       public async Task<Core.Domain.Entities.MachineGroup> GetByIdAsync(int id)
        {            
            const string query = @"
                SELECT 
                    Id,  GroupName,DepartmentId,Manufacturer, IsActive, IsDeleted,CreatedBy, CreatedDate, CreatedByName,CreatedIP
                FROM Maintenance.MachineGroup          
                WHERE Id = @id AND IsDeleted = 0";
                                
            return await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.MachineGroup>(query, new { id });
        }
        public async Task<bool> GetByMachineGroupCodeAsync(string groupName, int id)
        {
            var query = """
            SELECT COUNT(1) FROM Maintenance.MachineGroup
            WHERE GroupName = @GroupName AND IsDeleted = 0 AND Id <> @Id
            """;

            var result = await _dbConnection.ExecuteScalarAsync<int>(query, new { GroupName = groupName, Id = id });

            return result > 0;
        }
   

        public async Task<bool> GetByMachineGroupnameAsync(string groupName)
        {
            var query = """
            SELECT COUNT(1) FROM Maintenance.MachineGroup
            WHERE GroupName = @GroupName AND IsDeleted = 0 
            """;

            var result = await _dbConnection.ExecuteScalarAsync<int>(query, new { GroupName = groupName });

            return result > 0;
        }
         public async Task<bool> NotFoundAsync(int id)
        {
             var query = "SELECT COUNT(1) FROM Maintenance.MachineGroup WHERE Id = @Id AND IsDeleted = 0";
             
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = id });
                return count > 0;
        }   
        public async Task<(List<Core.Domain.Entities.MachineGroup>, int)> GetAllMachineGroupsAsync(int PageNumber, int PageSize, string? SearchTerm)
            {
                var query = $$"""
                DECLARE @TotalCount INT;
                SELECT @TotalCount = COUNT(*) 
                FROM [Maintenance].[MachineGroup] M
                WHERE M.IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (M.GroupName LIKE @Search)")}}; 

                SELECT M.Id, M.GroupName, M.Manufacturer,M.DepartmentId,M.IsActive, M.IsDeleted, 
                    M.CreatedBy, M.CreatedDate, M.CreatedByName, M.CreatedIP, 
                    M.ModifiedBy, M.ModifiedDate, M.ModifiedByName, M.ModifiedIP
                FROM [Maintenance].[MachineGroup] M
                WHERE M.IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (M.GroupName LIKE @Search)")}}
                ORDER BY M.Id DESC 
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
                
                // Read the data for MachineGroup and convert to list
                var machineGroupList = (await result.ReadAsync<Core.Domain.Entities.MachineGroup>()).ToList();
                
                // Read the total count
                int totalCount = await result.ReadFirstAsync<int>();

                return (machineGroupList, totalCount);
            }
             public async Task<List<Core.Domain.Entities.MachineGroup>> GetMachineGroupAutoComplete(string searchPattern)
               {
                   const string query = @"
                       SELECT Id, GroupName  
                       FROM Maintenance.MachineGroup
                       WHERE IsDeleted = 0 AND GroupName LIKE @SearchPattern";

                   var parameters = new 
                   { 
                       SearchPattern = $"%{searchPattern ?? string.Empty}%"
                   };
               var machineGroups = await _dbConnection.QueryAsync<Core.Domain.Entities.MachineGroup>(query, parameters);
                   return machineGroups.ToList();
               }  
            public async Task<bool> FKColumnExistValidation(int machineGroupId)
            {
                var sql = "SELECT COUNT(1) FROM Maintenance.MachineGroup WHERE Id = @Id AND IsDeleted = 0 AND IsActive = 1";
                  var count = await _dbConnection.ExecuteScalarAsync<int>(sql, new { Id = machineGroupId });
                  return count > 0;
            }        

    }
}