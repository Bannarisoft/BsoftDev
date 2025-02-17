
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IDepreciationGroup
{
    public interface IDepreciationGroupCommandRepository
    {
        Task<DepreciationGroups> CreateAsync(DepreciationGroups depreciationGroup);
        Task<int>  UpdateAsync(int depGroupId,DepreciationGroups depreciationGroup);
        Task<int>  DeleteAsync(int depGroupId,DepreciationGroups depreciationGroup);            
        Task<bool> ExistsByCodeAsync(string code );
        Task<int> GetMaxSortOrderAsync();
        Task<(bool IsNameDuplicate, bool IsSortOrderDuplicate)> CheckForDuplicatesAsync(string name, int sortOrder, int excludeId); 
    }
}