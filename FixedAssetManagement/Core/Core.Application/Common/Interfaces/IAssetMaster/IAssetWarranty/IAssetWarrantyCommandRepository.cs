using Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetWarranty
{
    public interface IAssetWarrantyCommandRepository
    {
        Task<AssetWarranties> CreateAsync(AssetWarranties assetWarranty);
        Task<int>  UpdateAsync(int assetId,AssetWarranties assetWarranty);
        Task<int>  DeleteAsync(int assetId,AssetWarranties assetWarranty);        
        Task<bool> ExistsByAssetIdAsync(int? assetId); 
        Task<AssetWarrantyDTO?> GetByAssetCodeAsync(string  assetCode);
        Task<bool> UpdateAssetWarrantyImageAsync(int assetId, string imageName);
         Task<AssetWarrantyDTO?> GetByAssetWarrantyAsync(string? assetCode);
        Task<bool> RemoveAssetWarrantyAsync(int assetId);
    }
}