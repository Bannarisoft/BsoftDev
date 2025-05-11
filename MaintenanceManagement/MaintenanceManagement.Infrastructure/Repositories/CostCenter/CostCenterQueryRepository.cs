using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.ICostCenter;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.CostCenter
{
    public class CostCenterQueryRepository :ICostCenterQueryRepository
    {
        private readonly IDbConnection _dbConnection; 
        private readonly IIPAddressService _ipAddressService;  

        public CostCenterQueryRepository(IDbConnection dbConnection, IIPAddressService ipAddressService)
        {
            _dbConnection = dbConnection;
            _ipAddressService = ipAddressService;
        }

        public async Task<(List<Core.Domain.Entities.CostCenter>, int)> GetAllCostCenterGroupAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var UnitId = _ipAddressService.GetUnitId();
            var query = $$"""
             DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
               FROM Maintenance.CostCenter
              WHERE IsDeleted = 0  AND UnitId = @UnitId
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CostCenterName LIKE @Search OR CostCenterCode LIKE @Search)")}};

                SELECT 
                Id, 
                CostCenterCode,
                CostCenterName,
                UnitId,
                DepartmentId,
                EffectiveDate,
                ResponsiblePerson,
                BudgetAllocated,
                Remarks,
                IsActive
            FROM Maintenance.CostCenter 
            WHERE 
            IsDeleted = 0
            AND UnitId = @UnitId
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CostCenterName LIKE @Search OR CostCenterCode LIKE @Search )")}}
                ORDER BY Id desc
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                SELECT @TotalCount AS TotalCount;
            """;

            
             var parameters = new
                       {
                           Search = $"%{SearchTerm}%",
                           Offset = (PageNumber - 1) * PageSize,
                           PageSize,
                           UnitId
                       };

             var costCenter = await _dbConnection.QueryMultipleAsync(query, parameters);
             var costCenterslist = (await costCenter.ReadAsync<Core.Domain.Entities.CostCenter>()).ToList();
             int totalCount = (await costCenter.ReadFirstAsync<int>());
             return (costCenterslist, totalCount);
        }

        public async Task<Core.Domain.Entities.CostCenter?> GetByIdAsync(int Id)
        {
            var UnitId = _ipAddressService.GetUnitId();
            const string query = @"
                    SELECT * 
                    FROM Maintenance.CostCenter 
                    WHERE Id = @Id AND IsDeleted = 0 AND UnitId = @UnitId"; 

                    var costCenter = await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.CostCenter>(query, new { Id, UnitId });
                    return costCenter;
        }

        public async Task<List<Core.Domain.Entities.CostCenter>> GetCostCenterGroups(string searchPattern)
        {
            var UnitId = _ipAddressService.GetUnitId();
            searchPattern = searchPattern ?? string.Empty; // Prevent null issues

            const string query = @"
             SELECT Id, CostCenterName 
            FROM Maintenance.CostCenter 
            WHERE IsDeleted = 0  AND UnitId = @UnitId
            AND CostCenterName LIKE @SearchPattern";  
            var parameters = new 
            { 
            SearchPattern = $"%{searchPattern}%" ,
            UnitId
            };

            var costCenters = await _dbConnection.QueryAsync<Core.Domain.Entities.CostCenter>(query, parameters);
            return costCenters.ToList();
        }

        public async Task<bool> SoftDeleteValidation(int Id)
        {
            const string query = @"
                           SELECT 1 
                           FROM [Maintenance].[MachineMaster]
                           WHERE CostCenterId = @Id AND IsDeleted = 0;";
                    
                       using var multi = await _dbConnection.QueryMultipleAsync(query, new { Id = Id });
                    
                       var costcentermasterDetailExists = await multi.ReadFirstOrDefaultAsync<int?>();
                    
                       return costcentermasterDetailExists.HasValue;
        }

       
    }
}