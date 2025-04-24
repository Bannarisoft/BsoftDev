using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IStcokLedger;
using Core.Application.StockLedger.Queries.GetCurrentStock;
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
        public async Task<CurrentStockDto?> GetSubStoresCurrentStock(string OldUnitcode, string Itemcode)
        {
            const string query = @"
                SELECT 
                    Oldunitcode as OldUnitId,
                    ItemCode,
                    ItemName,
                    SUM(ReceivedQty) - SUM(IssueQty) AS StockQty,
                    Uom,
                    SUM(ReceivedValue) - SUM(IssueValue) AS StockValue
                FROM 
                    Maintenance.StockLedger
                WHERE
                    Oldunitcode = @OldUnitcode AND
                    ItemCode = @Itemcode
                GROUP BY 
                    ItemCode, ItemName, Oldunitcode,Uom
                HAVING
                    SUM(ReceivedQty) - SUM(IssueQty) > 0";

            var parameters = new { OldUnitcode = OldUnitcode, Itemcode = Itemcode };

            var stocklist = await _dbConnection.QueryFirstOrDefaultAsync<CurrentStockDto>(query, parameters);

            return stocklist;
        }
    }
}