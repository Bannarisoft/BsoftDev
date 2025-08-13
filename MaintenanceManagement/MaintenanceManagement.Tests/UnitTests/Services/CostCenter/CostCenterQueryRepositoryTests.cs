using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.ICostCenter;
using Dapper;

namespace MaintenanceManagement.Tests.UnitTests.Services.CostCenter
{
    public class CostCenterQueryRepository : ICostCenterQueryRepository
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
              WHERE IsDeleted = 0
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
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CostCenterName LIKE @Search OR CostCenterCode LIKE @Search )")}}
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

            using var grid = await _dbConnection.QueryMultipleAsync(query, parameters);
            var costCenterslist = (await grid.ReadAsync<Core.Domain.Entities.CostCenter>()).ToList();
            int totalCount = await grid.ReadFirstAsync<int>();
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
            searchPattern ??= string.Empty;

            const string query = @"
             SELECT Id, CostCenterName 
            FROM Maintenance.CostCenter 
            WHERE IsDeleted = 0  AND UnitId = @UnitId
            AND CostCenterName LIKE @SearchPattern";

            var parameters = new
            {
                SearchPattern = $"%{searchPattern}%",
                UnitId
            };

            var costCenters = await _dbConnection.QueryAsync<Core.Domain.Entities.CostCenter>(query, parameters);
            return costCenters.ToList();
        }

        public async Task<bool> SoftDeleteValidation(int Id)
        {
            const string query = @"
                SELECT CASE WHEN EXISTS (
                    SELECT 1 
                    FROM [Maintenance].[MachineMaster]
                    WHERE CostCenterId = @Id AND IsDeleted = 0
                ) THEN 1 ELSE 0 END;";

            var exists = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id });
            return exists == 1;
        }

        public async Task<bool> DepartmentSoftDeleteValidation(int id)
        {
            const string query = @"
                SELECT CASE WHEN
                    EXISTS (SELECT 1 FROM [Maintenance].[CostCenter]                WHERE DepartmentId = @Id AND IsDeleted = 0) OR
                    EXISTS (SELECT 1 FROM [Maintenance].[MachineGroup]              WHERE DepartmentId = @Id AND IsDeleted = 0) OR
                    EXISTS (SELECT 1 FROM [Maintenance].[Feeder]                    WHERE DepartmentId = @Id AND IsDeleted = 0) OR
                    EXISTS (SELECT 1 FROM [Maintenance].[MachineGroupUser]          WHERE DepartmentId = @Id AND IsDeleted = 0) OR
                    EXISTS (SELECT 1 FROM [Maintenance].[MaintenanceRequest]        WHERE MaintenanceDepartmentId = @Id AND IsDeleted = 0) OR
                    EXISTS (SELECT 1 FROM [Maintenance].[MaintenanceRequest]        WHERE ProductionDepartmentId  = @Id AND IsDeleted = 0) OR
                    EXISTS (SELECT 1 FROM [Maintenance].[WorkCenter]                WHERE DepartmentId = @Id AND IsDeleted = 0) OR
                    EXISTS (SELECT 1 FROM [Maintenance].[PreventiveSchedulerHeader] WHERE DepartmentId = @Id AND IsDeleted = 0) OR
                    EXISTS (SELECT 1 FROM [Maintenance].[ActivityMaster]            WHERE DepartmentId = @Id AND IsDeleted = 0) OR
                    EXISTS (SELECT 1 FROM [Maintenance].[StockLedger]               WHERE DepartmentId = @Id)
                THEN 1 ELSE 0 END;";

            var any = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = id });
            return any == 1;
        }
    }
}
