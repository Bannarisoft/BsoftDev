using Core.Domain.Entities.AssetMaster;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetSpecification
{
    public interface IAssetSpecificationCommandRepository
    {
        Task<AssetSpecifications> CreateAsync(AssetSpecifications assetSpecification);
        Task<int>  UpdateAsync(int assetId,AssetSpecifications assetSpecification);
        Task<int>  DeleteAsync(int assetId,AssetSpecifications assetSpecification);        
        Task<bool> ExistsByAssetSpecIdAsync(int? assetId,int? assetSpecId); 
    }
}