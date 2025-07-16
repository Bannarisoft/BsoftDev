namespace Core.Application.Common.Interfaces.IAssetSubGroup
{
    public interface IAssetSubGroupCommandRepository
    {
         Task<int> CreateAsync(Core.Domain.Entities.AssetSubGroup assetSubGroup);
         Task<bool> ExistsByCodeAsync(string code );
         Task<bool> ExistsAsync(int groupId);
         Task<int> GetMaxSortOrderAsync();
         Task<int> UpdateAsync(int Id,Core.Domain.Entities.AssetSubGroup assetSubGroup);
         Task<(bool IsNameDuplicate, bool IsSortOrderDuplicate)> CheckForDuplicatesAsync(string name, int sortOrder, int excludeId);   
         Task<int> DeleteAsync(int Id,Core.Domain.Entities.AssetSubGroup assetSubGroup);
   
    }
}