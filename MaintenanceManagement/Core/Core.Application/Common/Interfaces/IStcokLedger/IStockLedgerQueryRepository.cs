using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.StockLedger.Queries.GetCurrentStock;
using Core.Application.StockLedger.Queries.GetCurrentStockItemsById;
using Core.Application.StockLedger.Queries.GetStockLegerReport;

namespace Core.Application.Common.Interfaces.IStcokLedger
{
    public interface IStockLedgerQueryRepository
    {
         Task<CurrentStockDto?> GetSubStoresCurrentStock(string OldUnitcode,string Itemcode,int DepartmentId);
         Task<List<StockItemCodeDto>> GetStockItemCodes(string OldUnitcode,int DepartmentId);
     
         
         
    }
}