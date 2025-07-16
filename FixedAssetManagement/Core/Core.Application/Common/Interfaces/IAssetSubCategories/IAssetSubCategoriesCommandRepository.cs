using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IAssetSubCategories
{
    public interface IAssetSubCategoriesCommandRepository
    {
         Task<int> CreateAsync(Core.Domain.Entities.AssetSubCategories assetSubCategories);
         Task<bool> ExistsByCodeAsync(string code );
         Task<int> GetMaxSortOrderAsync();
         Task<int> UpdateAsync(int Id,Core.Domain.Entities.AssetSubCategories assetSubCategories);
         Task<(bool IsNameDuplicate, bool IsSortOrderDuplicate)> CheckForDuplicatesAsync(string name, int sortOrder, int excludeId);   
         Task<int> DeleteAsync(int Id,Core.Domain.Entities.AssetSubCategories assetSubCategories);
    }
}