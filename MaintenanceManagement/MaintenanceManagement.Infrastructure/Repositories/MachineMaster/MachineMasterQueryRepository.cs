using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IMachineMaster;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.MachineMaster
{
    public class MachineMasterQueryRepository : IMachineMasterQueryRepository
    {
         private readonly IDbConnection _dbConnection; 
         private readonly IIPAddressService _ipAddressService;  
         public MachineMasterQueryRepository(IDbConnection dbConnection, IIPAddressService ipAddressService)
        {
            _dbConnection = dbConnection;
            _ipAddressService = ipAddressService;
        }
        public async Task<(List<Core.Domain.Entities.MachineMaster>, int)> GetAllMachineAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var UnitId = _ipAddressService.GetUnitId();
            var query = $$"""
             DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
             FROM Maintenance.MachineMaster
             WHERE IsDeleted = 0 AND UnitId = @UnitId
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (MachineCode LIKE @Search OR MachineName LIKE @Search)")}};

                SELECT 
                Id, 
                MachineCode,
                MachineName,
                MachineGroupId,
                UnitId,
                DepartmentId,
                ProductionCapacity,
                UomId,
                ShiftMasterId,
                CostCenterId,
                WorkCenterId,
                InstallationDate,
                AssetId,
                IsActive
            FROM Maintenance.MachineMaster 
            WHERE 
            IsDeleted = 0 AND UnitId = @UnitId
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (MachineCode LIKE @Search OR MachineName LIKE @Search )")}}
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

             var maintenanceCategory = await _dbConnection.QueryMultipleAsync(query, parameters);
             var maintenanceCategorylist = (await maintenanceCategory.ReadAsync<Core.Domain.Entities.MachineMaster>()).ToList();
             int totalCount = (await maintenanceCategory.ReadFirstAsync<int>());
             return (maintenanceCategorylist, totalCount);
        }

        public async Task<Core.Domain.Entities.MachineMaster?> GetByIdAsync(int Id)
        {
             var UnitId = _ipAddressService.GetUnitId();
             const string query = @"
                    SELECT * 
                    FROM Maintenance.MachineMaster 
                    WHERE Id = @Id AND IsDeleted = 0 AND UnitId = @UnitId";

                    var machineMaster = await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.MachineMaster>(query, new { Id,UnitId });
                    return machineMaster;
        }

        public async Task<List<Core.Domain.Entities.MachineMaster>> GetMachineAsync(string searchPattern)
        {
            var UnitId = _ipAddressService.GetUnitId();
            searchPattern = searchPattern ?? string.Empty; // Prevent null issues

            const string query = @"
             SELECT Id, MachineName 
            FROM Maintenance.MachineMaster 
            WHERE IsDeleted = 0 AND UnitId = @UnitId
            AND MachineName LIKE @SearchPattern";  
            var parameters = new 
            { 
            SearchPattern = $"%{searchPattern}%" ,
            UnitId
            };

            var machineMasters = await _dbConnection.QueryAsync<Core.Domain.Entities.MachineMaster>(query, parameters);
            return machineMasters.ToList();
        }

        public async Task<List<Core.Domain.Entities.MachineMaster>> GetMachineByGroupAsync(int MachineGroupId)
        {
            var UnitId = _ipAddressService.GetUnitId();
             const string query = @"
                    SELECT Id 
                    FROM Maintenance.MachineMaster 
                    WHERE MachineGroupId = @MachineGroupId AND IsDeleted = 0 AND UnitId = @UnitId";

                    var machineMaster = await _dbConnection.QueryAsync<Core.Domain.Entities.MachineMaster>(query, new { MachineGroupId, UnitId });
                    return machineMaster.ToList();
        }
        // public async Task<List<Core.Domain.Entities.MachineMaster>> GetMachineByGroup(int MachineGroupId)
        // {
        //     searchPattern = searchPattern ?? string.Empty; // Prevent null issues

        //     const string query = @"
        //      SELECT M.Id, M.MachineName 
        //     FROM Maintenance.MachineMaster M
        //     LEFT JOIN [Maintenance].[WorkOrder] WO on WO.MachineId=M.Id
        //     WHERE IsDeleted = 0 
        //     AND MachineName LIKE @SearchPattern";  
        //     var parameters = new 
        //     { 
        //     SearchPattern = $"%{searchPattern}%" 
        //     };

        //     var machineMasters = await _dbConnection.QueryAsync<Core.Domain.Entities.MachineMaster>(query, parameters);
        //     return machineMasters.ToList();
        // }
        public async Task<List<Core.Domain.Entities.MachineMaster>> GetMachineByGroupSagaAsync(int MachineGroupId,int UnitId)
        {
            
             const string query = @"
                    SELECT Id 
                    FROM Maintenance.MachineMaster 
                    WHERE MachineGroupId = @MachineGroupId AND IsDeleted = 0 AND UnitId = @UnitId";

                    var machineMaster = await _dbConnection.QueryAsync<Core.Domain.Entities.MachineMaster>(query, new { MachineGroupId, UnitId });
                    return machineMaster.ToList();
        }
    }
}