using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral
{
    public interface IAssetMasterGeneralCommandRepository
    {
        //Task<AssetMasterGenerals> CreateAsync(AssetMasterGenerals assetMasterGeneral);
        Task<AssetMasterGenerals> CreateAsync(AssetMasterGenerals assetMasterGeneral, CancellationToken cancellationToken);
        Task<int>  UpdateAsync(int depGroupId,AssetMasterGenerals assetMasterGeneral);
        Task<int>  DeleteAsync(int depGroupId,AssetMasterGenerals assetMasterGeneral);        
        Task<string?> GetAssetGroupNameById(int assetGroupId);
        Task<string?> GetAssetCategoryNameById(int assetCategoryId);
        Task<string?> GetLatestAssetCode(int companyId,int unitId, int assetGroupId, int assetSubCategoryId);
        Task<AssetMasterGenerals?> GetByAssetCodeAsync(string assetCode);
        Task<bool> UpdateAssetImageAsync(int assetId, string imageName);
        Task<AssetMasterGeneralDTO?> GetByAssetImageAsync(string assetCode);
        Task<bool> RemoveAssetImageReferenceAsync(int assetId);
        // Add a transaction method
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);    
        Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken);

        
    }
}