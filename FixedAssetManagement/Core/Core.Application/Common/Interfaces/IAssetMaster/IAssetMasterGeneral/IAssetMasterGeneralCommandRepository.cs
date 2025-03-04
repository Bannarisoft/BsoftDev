using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Domain.Entities;
using Core.Domain.Entities.AssetPurchase;
using Microsoft.EntityFrameworkCore.Storage;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral
{
    public interface IAssetMasterGeneralCommandRepository
    {
        Task<AssetMasterGenerals> CreateAsync(AssetMasterGenerals assetMasterGeneral, CancellationToken cancellationToken);
        Task<int>  UpdateAsync(int depGroupId,AssetMasterGenerals assetMasterGeneral);
        Task<int>  DeleteAsync(int depGroupId,AssetMasterGenerals assetMasterGeneral);        
        Task<AssetMasterGenerals?> GetByAssetCodeAsync(string assetCode);
        Task<bool> UpdateAssetImageAsync(int assetId, string imageName);
        Task<AssetMasterGeneralDTO?> GetByAssetImageAsync(string assetCode);
        Task<bool> RemoveAssetImageReferenceAsync(int assetId);        
    }
}