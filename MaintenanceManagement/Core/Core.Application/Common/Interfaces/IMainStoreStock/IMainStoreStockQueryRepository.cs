using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.MainStoreStock.Queries.GetItemStockbyId;
using Core.Application.MainStoreStock.Queries.GetMainStoreStock;
using Core.Application.MainStoreStock.Queries.GetMainStoreStockItems;

namespace Core.Application.Common.Interfaces.IMainStoreStock
{
    public interface IMainStoreStockQueryRepository
    {
        Task<List<MainStoresStockDto>> GetStockDetails(string OldUnitcode,string GroupCode);
        Task<List<MainStoresStockItemsDto>> GetStockItemsCodes(string OldUnitcode,string GroupCode);
        Task<MainStoreItemStockDto?> GetByItemCodeIdAsync(string OldUnitcode,string ItemCode);
    }
}