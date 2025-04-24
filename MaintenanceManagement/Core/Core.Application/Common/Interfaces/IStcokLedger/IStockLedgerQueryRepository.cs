using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.StockLedger.Queries.GetCurrentStock;

namespace Core.Application.Common.Interfaces.IStcokLedger
{
    public interface IStockLedgerQueryRepository
    {
         Task<CurrentStockDto?> GetSubStoresCurrentStock(string OldUnitcode,string Itemcode);
         
    }
}