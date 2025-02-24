using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral
{
    public interface IAssetMasterGeneralQueryRepository
    {
        Task<AssetMasterGeneralDTO>  GetByIdAsync(int assetId);
        Task<(List<AssetMasterGeneralDTO>,int)> GetAllAssetAsync(int PageNumber, int PageSize, string? SearchTerm);        
        Task<List<AssetMasterGeneralDTO>> GetByAssetNameAsync(string assetName);    
        Task<List<Core.Domain.Entities.MiscMaster>> GetAssetTypeAsync();     
        Task<List<Core.Domain.Entities.MiscMaster>> GetWorkingStatusAsync();          
    }
}