using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetLocation
{
    public interface IAssetLocationCommandRepository
    {
        Task<Core.Domain.Entities.AssetMaster.AssetLocation> CreateAsync(Core.Domain.Entities.AssetMaster.AssetLocation assetLocation); 

         Task<bool>  UpdateAsync(int Id,Core.Domain.Entities.AssetMaster.AssetLocation assetLocation);
    }
}