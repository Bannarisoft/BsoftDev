using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral
{
    public interface IAssetMasterGeneralQueryRepository
    {
        Task<AssetMasterGenerals>  GetByIdAsync(int assetId);
        Task<(List<AssetMasterGenerals>,int)> GetAllAssetAsync(int PageNumber, int PageSize, string? SearchTerm);        
        Task<List<AssetMasterGenerals>> GetByAssetNameAsync(string assetName);         
    }
}