using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IMaintenanceCategory;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.MaintenanceCategory
{
    public class MaintenanceCategoryQueryRepository : IMaintenanceCategoryQueryRepository
    {
        private readonly IDbConnection _dbConnection; 
         public MaintenanceCategoryQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<(List<Core.Domain.Entities.MaintenanceCategory>, int)> GetAllMaintenanceCategoryAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
             var query = $$"""
             DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
             FROM Maintenance.MaintenanceCategory
             WHERE IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CategoryName LIKE @Search OR Id LIKE @Search)")}};

                SELECT 
                Id, 
                CategoryName,
                Description,
                IsActive,CreatedDate
            FROM Maintenance.MaintenanceCategory 
            WHERE 
            IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CategoryName LIKE @Search OR Id LIKE @Search )")}}
                ORDER BY Id desc
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                SELECT @TotalCount AS TotalCount;
            """;

            
             var parameters = new
                       {
                           Search = $"%{SearchTerm}%",
                           Offset = (PageNumber - 1) * PageSize,
                           PageSize
                       };

             var maintenanceCategory = await _dbConnection.QueryMultipleAsync(query, parameters);
             var maintenanceCategorylist = (await maintenanceCategory.ReadAsync<Core.Domain.Entities.MaintenanceCategory>()).ToList();
             int totalCount = (await maintenanceCategory.ReadFirstAsync<int>());
             return (maintenanceCategorylist, totalCount);
        }

        public async Task<Core.Domain.Entities.MaintenanceCategory?> GetByIdAsync(int Id)
        {
             const string query = @"
                    SELECT * 
                    FROM Maintenance.MaintenanceCategory 
                    WHERE Id = @Id AND IsDeleted = 0";

                    var maintenanceCategory = await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.MaintenanceCategory>(query, new { Id });
                    return maintenanceCategory;
        }

        public async Task<List<Core.Domain.Entities.MaintenanceCategory>> GetMaintenanceCategoryAsync(string searchPattern)
        {
            searchPattern = searchPattern ?? string.Empty; // Prevent null issues

            const string query = @"
             SELECT Id, CategoryName 
            FROM Maintenance.MaintenanceCategory 
            WHERE IsDeleted = 0 
            AND CategoryName LIKE @SearchPattern";  
            var parameters = new 
            { 
            SearchPattern = $"%{searchPattern}%" 
            };

            var maintenanceCategories = await _dbConnection.QueryAsync<Core.Domain.Entities.MaintenanceCategory>(query, parameters);
            return maintenanceCategories.ToList();
        }
    }
}