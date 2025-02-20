using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetSpecification
{
    public interface IAssetSpecificationQueryRepository
    {
        Task<AssetSpecificationDTO>  GetByIdAsync(int assetId);
        Task<(List<AssetSpecificationDTO>,int)> GetAllAssetSpecificationAsync(int PageNumber, int PageSize, string? SearchTerm);        
        Task<List<AssetSpecificationDTO>> GetByAssetSpecificationNameAsync(string assetName);    

    }
}