using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Domain.Entities;


namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral
{
    public interface IAssetMasterGeneralCommandRepository
    {
        Task<AssetMasterGenerals> CreateAsync(AssetMasterGenerals assetMasterGeneral, CancellationToken cancellationToken);
        Task<bool>  UpdateAsync(int assetId,AssetMasterGenerals assetMasterGeneral);
        Task<bool>  DeleteAsync(int assetId,AssetMasterGenerals assetMasterGeneral);        
        Task<AssetMasterGenerals?> GetByAssetCodeAsync(string assetCode);
        Task<bool> UpdateAssetImageAsync(int assetId, string imageName);
        Task<AssetMasterGeneralDTO?> GetByAssetImageAsync(string imageName);
        Task<bool> RemoveAssetImageReferenceAsync(string assetId);
        Task<bool> RemoveAssetWarrantyAsync(string assetPath);    
        Task<bool> RemoveAssetDocumentReferenceAsync(string assetId);   
        Task<bool> UpdateAssetDocumentAsync(int assetId, string imageName); 
        Task<bool> UpdateDocumentAsync(int AssetId, string imageName);      
    }
}