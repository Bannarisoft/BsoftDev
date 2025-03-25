using Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetWarranty
{
    public interface IAssetWarrantyQueryRepository
    {
        Task<AssetWarrantyDTO>  GetByIdAsync(int assetId);
        Task<(List<AssetWarrantyDTO>,int)> GetAllAssetWarrantyAsync(int PageNumber, int PageSize, string? SearchTerm);        
        Task<List<AssetWarrantyDTO>> GetByAssetWarrantyNameAsync(string assetName);  
        Task<List<Core.Domain.Entities.MiscMaster>> GetWarrantyTypeAsync();    
        Task<List<Core.Domain.Entities.MiscMaster>> GetWarrantyClaimStatusAsync();  
        Task<bool> SoftDeleteValidation(int Id);   
    }
}