
using Core.Domain.Common;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

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
        Task<bool> CheckForDuplicatesAsync(int groupId, int depMethodId,int bookTypeId,BaseEntity.Status isActive,string Code,string name,int excludeId);        
    }
}