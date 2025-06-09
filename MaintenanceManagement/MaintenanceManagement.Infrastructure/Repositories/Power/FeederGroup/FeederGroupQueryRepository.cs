using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.Power.IFeederGroup;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.Power.FeederGroup
{
    public class FeederGroupQueryRepository : IFeederGroupQueryRepository
    {

        private readonly IDbConnection _dbConnection;

        public FeederGroupQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<(List<Core.Domain.Entities.Power.FeederGroup>, int)> GetAllFeederGroupAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var query = $$"""
            DECLARE @TotalCount INT;
            SELECT @TotalCount = COUNT(*) 
            FROM [Maintenance].[FeederGroup] FG
            WHERE FG.IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (FG.FeederGroupName LIKE @Search OR FG.FeederGroupCode LIKE @Search)")}}; 

            SELECT FG.Id, FG.FeederGroupCode, FG.FeederGroupName, FG.IsActive, FG.IsDeleted, 
                FG.CreatedBy, FG.CreatedDate, FG.CreatedByName, FG.CreatedIP, 
                FG.ModifiedBy, FG.ModifiedDate, FG.ModifiedByName, FG.ModifiedIP
            FROM [Maintenance].[FeederGroup] FG
            WHERE FG.IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (FG.FeederGroupName LIKE @Search OR FG.FeederGroupCode LIKE @Search)")}}
            ORDER BY FG.Id DESC 
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

            var feederGroupList = (await result.ReadAsync<Core.Domain.Entities.Power.FeederGroup>()).ToList();
            int totalCount = await result.ReadFirstAsync<int>();

            return (feederGroupList, totalCount);
        }
        public async Task<Core.Domain.Entities.Power.FeederGroup> GetFeederGroupByIdAsync(int id)
        {
            var query = """
                SELECT FG.Id, FG.FeederGroupCode, FG.FeederGroupName, FG.IsActive, FG.IsDeleted, 
                    FG.CreatedBy, FG.CreatedDate, FG.CreatedByName, FG.CreatedIP, 
                    FG.ModifiedBy, FG.ModifiedDate, FG.ModifiedByName, FG.ModifiedIP
                FROM [Maintenance].[FeederGroup] FG
                WHERE FG.IsDeleted = 0 AND FG.Id = @Id;
                """;

            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.Power.FeederGroup>(query, new { Id = id });
            return result.FirstOrDefault();
        }

        public async Task<bool> AlreadyExistsAsync(string feederGroupCode, int? id = null)
        {

            var query = "SELECT COUNT(1) FROM [Maintenance].[FeederGroup] WHERE FeederGroupCode = @feederGroupCode AND IsDeleted = 0";
            var parameters = new DynamicParameters(new { FeederGroupCode = feederGroupCode });

            if (id is not null)
            {
                query += " AND Id != @Id";
                parameters.Add("Id", id);
            }
            var count = await _dbConnection.ExecuteScalarAsync<int>(query, parameters);
            return count > 0;
        }


        public async Task<bool> NotFoundAsync(int id)
        {
            var query = "SELECT COUNT(1) FROM Maintenance.FeederGroup WHERE Id = @Id AND IsDeleted = 0";

            var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = id });
            return count > 0;
        }  
        
         public async Task<List<Core.Domain.Entities.Power.FeederGroup>> GetFeederGroupAutoComplete(string searchPattern)
            {
                   const string query = @"
                       SELECT Id, FeederGroupCode,FeederGroupName  
                       FROM Maintenance.FeederGroup
                       WHERE IsDeleted = 0 AND (FeederGroupName LIKE @SearchPattern OR FeederGroupCode LIKE @SearchPattern)";
                   var parameters = new 
                   { 
                       SearchPattern = $"%{searchPattern ?? string.Empty}%"
                   };
               var feederGroups = await _dbConnection.QueryAsync<Core.Domain.Entities.Power.FeederGroup>(query, parameters);
               
                   return feederGroups.ToList();
            } 


    }
}