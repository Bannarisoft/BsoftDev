using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities.AssetPurchase;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetAdditionalCost
{
    public interface IAssetAdditionalCostCommandRepository
    {
          Task<int> CreateAsync(AssetAdditionalCost assetAdditionalCost);
          Task<int> UpdateAsync(int id,AssetAdditionalCost assetAdditionalCost);
    }
}