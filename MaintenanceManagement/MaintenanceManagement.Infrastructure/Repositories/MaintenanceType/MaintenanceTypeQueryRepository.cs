using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IMaintenanceType;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.MaintenanceType
{
    public class MaintenanceTypeQueryRepository : IMaintenanceTypeQueryRepository
    {
         private readonly IDbConnection _dbConnection; 
         public MaintenanceTypeQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<(List<Core.Domain.Entities.MaintenanceType>, int)> GetAllMaintenanceTypeAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
             var query = $$"""
             DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
             FROM Maintenance.MaintenanceType
             WHERE IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (TypeName LIKE @Search OR Id LIKE @Search)")}};

                SELECT 
                Id, 
                TypeName,
                IsActive
            FROM Maintenance.MaintenanceType 
            WHERE 
            IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (TypeName LIKE @Search OR Id LIKE @Search )")}}
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

             var maintenancetype = await _dbConnection.QueryMultipleAsync(query, parameters);
             var maintenancetypelist = (await maintenancetype.ReadAsync<Core.Domain.Entities.MaintenanceType>()).ToList();
             int totalCount = (await maintenancetype.ReadFirstAsync<int>());
             return (maintenancetypelist, totalCount);
        }

        public async Task<Core.Domain.Entities.MaintenanceType?> GetByIdAsync(int Id)
        {
             const string query = @"
                    SELECT * 
                    FROM Maintenance.MaintenanceType 
                    WHERE Id = @Id AND IsDeleted = 0";

                    var maintenanceType = await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.MaintenanceType>(query, new { Id });
                    return maintenanceType;
        }

        public async Task<List<Core.Domain.Entities.MaintenanceType>> GetMaintenanceTypeAsync(string searchPattern)
        {
             searchPattern = searchPattern ?? string.Empty; // Prevent null issues

            const string query = @"
             SELECT Id, TypeName 
            FROM Maintenance.MaintenanceType 
            WHERE IsDeleted = 0 
            AND TypeName LIKE @SearchPattern";  
            var parameters = new 
            { 
            SearchPattern = $"%{searchPattern}%" 
            };

            var maintenanceTypes = await _dbConnection.QueryAsync<Core.Domain.Entities.MaintenanceType>(query, parameters);
            return maintenanceTypes.ToList();
        }
    }
}