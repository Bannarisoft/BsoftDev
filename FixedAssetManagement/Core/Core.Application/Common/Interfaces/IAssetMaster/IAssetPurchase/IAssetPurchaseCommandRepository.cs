using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities.AssetPurchase;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetPurchase
{
    public interface IAssetPurchaseCommandRepository
    {
        Task<int> CreateAsync(AssetPurchaseDetails assetPurchaseDetails);
        Task<int> UpdateAsync(int Id,AssetPurchaseDetails assetPurchaseDetails);
    }
}