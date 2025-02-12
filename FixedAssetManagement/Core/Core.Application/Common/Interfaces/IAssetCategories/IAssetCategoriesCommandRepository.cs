using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IAssetCategories
{
    public interface IAssetCategoriesCommandRepository
    {
         Task<int> CreateAsync(Core.Domain.Entities.AssetCategories assetCategories);
         Task<bool> ExistsByCodeAsync(string code );
         Task<int> GetMaxSortOrderAsync();
         Task<int> UpdateAsync(int Id,Core.Domain.Entities.AssetCategories assetCategories);
         Task<(bool IsNameDuplicate, bool IsSortOrderDuplicate)> CheckForDuplicatesAsync(string name, int sortOrder, int excludeId);   
         Task<int> DeleteAsync(int Id,Core.Domain.Entities.AssetCategories assetCategories);
    }
}