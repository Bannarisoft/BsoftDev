using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IWorkCenter;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.WorkCenter
{
    public class WorkCenterQueryRepository : IWorkCenterQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IIPAddressService _ipAddressService;

        public WorkCenterQueryRepository(IDbConnection dbConnection, IIPAddressService ipAddressService)
        {
            _dbConnection = dbConnection;
            _ipAddressService = ipAddressService;
        }

        public async Task<(List<Core.Domain.Entities.WorkCenter>, int)> GetAllWorkCenterGroupAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var query = $$"""
             DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
               FROM Maintenance.WorkCenter
              WHERE IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (WorkCenterName LIKE @Search OR WorkCenterCode LIKE @Search)")}};

                SELECT 
                Id, 
                WorkCenterCode,
                WorkCenterName,
                UnitId,
                DepartmentId,
                IsActive,CreatedDate
            FROM Maintenance.WorkCenter 
            WHERE 
            IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (WorkCenterName LIKE @Search OR WorkCenterCode LIKE @Search )")}}
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

            var workCenter = await _dbConnection.QueryMultipleAsync(query, parameters);
            var workCenterslist = (await workCenter.ReadAsync<Core.Domain.Entities.WorkCenter>()).ToList();
            int totalCount = (await workCenter.ReadFirstAsync<int>());
            return (workCenterslist, totalCount);
        }

        public async Task<Core.Domain.Entities.WorkCenter?> GetByIdAsync(int Id)
        {
            var UnitId = _ipAddressService.GetUnitId();
            const string query = @"
                    SELECT * 
                    FROM Maintenance.WorkCenter 
                    WHERE Id = @Id AND IsDeleted = 0 AND UnitId = @UnitId";

            var workCenter = await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.WorkCenter>(query, new { Id, UnitId });
            return workCenter;
        }

        public async Task<List<Core.Domain.Entities.WorkCenter>> GetWorkCenterGroups(string searchPattern)
        {
            var UnitId = _ipAddressService.GetUnitId();
            searchPattern = searchPattern ?? string.Empty; // Prevent null issues

            const string query = @"
             SELECT Id, WorkCenterName 
            FROM Maintenance.WorkCenter 
            WHERE IsDeleted = 0 AND UnitId = @UnitId
            AND WorkCenterName LIKE @SearchPattern";
            var parameters = new
            {
                SearchPattern = $"%{searchPattern}%",
                UnitId
            };

            var workCenters = await _dbConnection.QueryAsync<Core.Domain.Entities.WorkCenter>(query, parameters);
            return workCenters.ToList();
        }

        public async Task<bool> SoftDeleteValidation(int Id)
        {
            const string query = @"
                           SELECT 1 
                           FROM [Maintenance].[MachineMaster]
                           WHERE WorkCenterId = @Id AND IsDeleted = 0;";

            using var multi = await _dbConnection.QueryMultipleAsync(query, new { Id = Id });

            var WorkCentermasterDetailExists = await multi.ReadFirstOrDefaultAsync<int?>();

            return WorkCentermasterDetailExists.HasValue;
        }
    }
}