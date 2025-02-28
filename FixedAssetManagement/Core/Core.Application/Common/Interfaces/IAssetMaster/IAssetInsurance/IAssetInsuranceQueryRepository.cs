using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetInsurance
{
    public interface IAssetInsuranceQueryRepository 
    {
            Task<Core.Domain.Entities.AssetMaster.AssetInsurance>  GetByAssetIdAsync(int id);

            
        
    }
}