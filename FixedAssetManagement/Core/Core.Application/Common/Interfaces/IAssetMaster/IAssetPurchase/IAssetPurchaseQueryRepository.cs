using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetPurchase
{
    public interface IAssetPurchaseQueryRepository
    {
        Task<List<Core.Domain.Entities.AssetSource>> GetAssetSources(string searchPattern);
        Task<List<Core.Domain.Entities.AssetPurchase.AssetUnit>> GetAssetUnit(string UserName);
        Task<List<Core.Domain.Entities.AssetPurchase.AssetGrn>> GetAssetGrnNo(int OldUnitId);
        Task<List<Core.Domain.Entities.AssetPurchase.AssetGrnItem>> GetAssetGrnItem(int OldUnitId, int GrnNo);
        Task<List<Core.Domain.Entities.AssetPurchase.AssetGrnDetails>> GetAssetGrnItemDetails(int OldUnitId, int GrnNo,int GrnSerialNo);
    
    }
    
}