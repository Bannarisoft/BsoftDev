using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IMainStoreStock;
using Core.Application.MainStoreStock.Queries.GetItemStockbyId;
using Core.Application.MainStoreStock.Queries.GetMainStoreStock;
using Core.Application.MainStoreStock.Queries.GetMainStoreStockItems;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.MainStoreStock
{
    public class MainStoreStockQueryRepository :IMainStoreStockQueryRepository
    {
         private readonly IDbConnection _dbConnection;
        public MainStoreStockQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<MainStoreItemStockDto?> GetByItemCodeIdAsync(string oldUnitcode, string itemCode)
        {
            oldUnitcode ??= string.Empty;
            itemCode ??= string.Empty;

            // Get procedure name dynamically
            var procedureName = await _dbConnection.QueryFirstOrDefaultAsync<string>(
                "SELECT StoredProcedureName FROM DivisionProcedureMapping WHERE OldUnitcode = @OldUnitcode AND Type = 'I'",
                new { OldUnitcode = oldUnitcode }
            );

            procedureName ??= "GetKalsoftePrimeMainstoreStock"; // Default procedure

            var parameters = new
            {
                DivCode = oldUnitcode,
                ItemCode = itemCode
            };

            var result = await _dbConnection.QueryAsync<MainStoreItemStockDto>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.FirstOrDefault(); // Get single or null
        }

        public async Task<List<MainStoresStockDto>> GetStockDetails(string OldUnitcode, string GroupCode)
        {
            OldUnitcode ??= string.Empty;
            GroupCode ??= string.Empty;

            // Get procedure name from database mapping table
            var procedureName = await _dbConnection.QueryFirstOrDefaultAsync<string>(
                "SELECT StoredProcedureName FROM DivisionProcedureMapping WHERE OldUnitcode = @OldUnitcode and Type='S'",
                new { OldUnitcode }
            );

            // If not found, use default procedure
            procedureName ??= "GetKalsoftePrimeMainstoreStock";

            var parameters = new
            {
                DivCode = OldUnitcode,
                FromGroupcode = GroupCode
            };

            // Call the procedure dynamically
            var itemcodes = await _dbConnection.QueryAsync<MainStoresStockDto>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return itemcodes.ToList();
        }

        public async Task<List<MainStoresStockItemsDto>> GetStockItemsCodes(string OldUnitcode, string GroupCode)
        {
            OldUnitcode ??= string.Empty;
            GroupCode ??= string.Empty;

            // Get procedure name from database mapping table
            var procedureName = await _dbConnection.QueryFirstOrDefaultAsync<string>(
                "SELECT StoredProcedureName FROM DivisionProcedureMapping WHERE OldUnitcode = @OldUnitcode and Type='S'",
                new { OldUnitcode }
            );

            // If not found, use default procedure
            procedureName ??= "GetKalsoftePrimeMainstoreStock";

            var parameters = new
            {
                DivCode = OldUnitcode,
                FromGroupcode = GroupCode
            };

            // Call the procedure dynamically
            var itemcodes = await _dbConnection.QueryAsync<MainStoresStockItemsDto>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return itemcodes.ToList();
        }
    }
}