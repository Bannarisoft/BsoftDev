using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral
{
    public interface IAssetMasterGeneralCommandRepository
    {
        Task<AssetMasterGenerals> CreateAsync(AssetMasterGenerals assetMasterGeneral);
        Task<int>  UpdateAsync(int depGroupId,AssetMasterGenerals assetMasterGeneral);
        Task<int>  DeleteAsync(int depGroupId,AssetMasterGenerals assetMasterGeneral);        
        Task<string?> GetAssetGroupNameById(int assetGroupId);
        Task<string?> GetAssetSubCategoryNameById(int assetSubCategoryId);
        Task<string?> GetLatestAssetCode(int companyId,int unitId, int assetGroupId, int assetSubCategoryId);

    }
}