
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IDepreciationGroup
{
    public interface IDepreciationGroupCommandRepository
    {
        Task<DepreciationGroups> CreateAsync(DepreciationGroups depreciationGroup);
        Task<bool>  UpdateAsync(DepreciationGroups depreciationGroup);
        Task<int>  DeleteAsync(int depGroupId,DepreciationGroups depreciationGroup); 
        Task<bool> ExistsByAssetGroupIdAsync(int assetGroupId); // ✅ New method           
        Task<bool> ExistsByCodeAsync(string code );
        Task<int> GetMaxSortOrderAsync();        
        Task<(bool IsNameDuplicate,bool IsCodeDuplicate, bool IsSortOrderDuplicate)> CheckForDuplicatesAsync(string name, string code, int sortOrder, int excludeId); 
    }
}