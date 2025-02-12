using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IAssetGroup
{
    public interface IAssetGroupCommandRepository
    {
         Task<int> CreateAsync(Core.Domain.Entities.AssetGroup assetGroup);
         Task<bool> ExistsByCodeAsync(string code );
         Task<int> GetMaxSortOrderAsync();
         Task<int> UpdateAsync(int Id,Core.Domain.Entities.AssetGroup assetGroup);
         Task<(bool IsNameDuplicate, bool IsSortOrderDuplicate)> CheckForDuplicatesAsync(string name, int sortOrder, int excludeId);   
         Task<int> DeleteAsync(int Id,Core.Domain.Entities.AssetGroup assetGroup);
   
    }
}