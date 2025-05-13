using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IStcokLedger;
using Core.Application.StockLedger.Queries.GetCurrentStock;
using Core.Application.StockLedger.Queries.GetCurrentStockItemsById;
using Core.Application.StockLedger.Queries.GetStockLegerReport;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.StockLedger
{
    public class StockLedgerQueryRepository : IStockLedgerQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        public StockLedgerQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<CurrentStockDto>> GetStockDetails(string OldUnitcode)
        {
             OldUnitcode = OldUnitcode ?? string.Empty; // Prevent null issues

            const string query = @"
                SELECT 
                    Oldunitcode as OldUnitId,
                    ItemCode,
                    ItemName,
					Uom,
                    SUM(ReceivedQty) - SUM(IssueQty) AS StockQty,
                    SUM(ReceivedValue) - SUM(IssueValue) AS StockValue
                FROM 
                    Maintenance.StockLedger
                WHERE
                    Oldunitcode = @OldUnitcode 
                    AND TransactionType not in('SRP')
                GROUP BY 
                    ItemCode, ItemName, Oldunitcode,Uom
                HAVING
                    SUM(ReceivedQty) - SUM(IssueQty) > 0";

            var parameters = new 
            { 
                OldUnitcode // match exactly, no wildcards
            };

            var itemcodes = await _dbConnection.QueryAsync<CurrentStockDto>(query, parameters);
            return itemcodes.ToList();
        }

        public async Task<List<StockItemCodeDto>> GetStockItemCodes(string OldUnitcode)
        {
            OldUnitcode = OldUnitcode ?? string.Empty; // Prevent null issues

            const string query = @"
                SELECT 
                    ItemCode,
                    ItemName
                FROM 
                    Maintenance.StockLedger
                WHERE
                    Oldunitcode = @OldUnitcode
                    AND TransactionType not in('SRP') 
                GROUP BY 
                    ItemCode, ItemName, Oldunitcode
                HAVING
                    SUM(ReceivedQty) - SUM(IssueQty) > 0";

            var parameters = new 
            { 
                OldUnitcode // match exactly, no wildcards
            };

            var itemcodes = await _dbConnection.QueryAsync<StockItemCodeDto>(query, parameters);
            return itemcodes.ToList();
        }

        public async Task<CurrentStockDto?> GetSubStoresCurrentStock(string OldUnitcode, string Itemcode)
        {
            const string query = @"
                SELECT 
                    Oldunitcode as OldUnitId,
                    ItemCode,
                    ItemName,
                    SUM(ReceivedQty) - SUM(IssueQty) AS StockQty,
                    Uom,
                    SUM(ReceivedValue) - SUM(IssueValue) AS StockValue,
                    ((SUM(ReceivedValue) - SUM(IssueValue)) / (SUM(ReceivedQty) - SUM(IssueQty))) AS Rate
                FROM 
                    Maintenance.StockLedger
                WHERE
                    Oldunitcode = @OldUnitcode AND
                    ItemCode = @Itemcode AND
                    TransactionType not in('SRP')
                GROUP BY 
                    ItemCode, ItemName, Oldunitcode,Uom
                HAVING
                    SUM(ReceivedQty) - SUM(IssueQty) > 0";

            var parameters = new { OldUnitcode = OldUnitcode, Itemcode = Itemcode };

            var stocklist = await _dbConnection.QueryFirstOrDefaultAsync<CurrentStockDto>(query, parameters);

            return stocklist;
        }

        public async Task<List<StockLedgerReportDto>> GetSubStoresStockLedger(string OldUnitcode, DateTime FromDate, DateTime ToDate, string? Itemcode)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FromDate", FromDate);
            parameters.Add("@ToDate", ToDate);
            parameters.Add("@ItemCode", Itemcode);
            parameters.Add("@OldUnitCode", OldUnitcode);

            var result = await _dbConnection.QueryAsync<StockLedgerReportDto>(
                "GetSubStoreStockLedgerSummary",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }
    }
}