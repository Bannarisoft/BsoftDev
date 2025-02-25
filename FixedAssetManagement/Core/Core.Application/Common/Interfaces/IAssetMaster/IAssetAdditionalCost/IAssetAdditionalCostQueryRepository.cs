using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetAdditionalCost
{
    public interface IAssetAdditionalCostQueryRepository
    {
         Task<List<Core.Domain.Entities.MiscMaster>> GetCostTypeAsync();  
         Task<Core.Domain.Entities.AssetPurchase.AssetAdditionalCost?> GetByIdAsync(int Id);
         Task<(List<Core.Domain.Entities.AssetPurchase.AssetAdditionalCost>,int)> GetAllAdditionalCostGroupAsync(int PageNumber, int PageSize, string? SearchTerm); 
    }
}