
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IExcelImport
{
    public interface  IExcelImportCommandRepository
    {
        Task AddRangeAsync(IEnumerable<AssetMasterGenerals> assets);
        Task SaveChangesAsync();
        Task<bool> ImportAssetsAsync(List<AssetMasterDto> assets, CancellationToken cancellationToken);
        Task<int?> GetAssetGroupIdByNameAsync(string assetGroupName); 
        Task<int?> GetAssetCategoryIdByNameAsync(string assetGroupName); 
        Task<int?> GetAssetSubCategoryIdByNameAsync(string assetGroupName); 
        Task<int?> GetAssetUOMIdByNameAsync(string assetGroupName);
        Task<int?> GetAssetLocationIdByNameAsync(string locationName);
        Task<int?> GetAssetSubLocationIdByNameAsync(string subLocationName);
    }
}