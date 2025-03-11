using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetDisposal
{
    public interface IAssetDisposalCommandRepository
    {
        Task<int> CreateAsync(AssetDisposal assetDisposal);
        Task<int> UpdateAsync(int id,AssetDisposal assetDisposal);
    }
}