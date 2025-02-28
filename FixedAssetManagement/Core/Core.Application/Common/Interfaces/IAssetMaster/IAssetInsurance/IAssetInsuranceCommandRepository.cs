using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetInsurance
{
    public interface IAssetInsuranceCommandRepository
    {
        Task<Core.Domain.Entities.AssetMaster.AssetInsurance> CreateAsync(Core.Domain.Entities.AssetMaster.AssetInsurance assetInsurance);   
        Task<bool> UpdateAsync(int id, Core.Domain.Entities.AssetMaster.AssetInsurance assetInsurance);
    }
}