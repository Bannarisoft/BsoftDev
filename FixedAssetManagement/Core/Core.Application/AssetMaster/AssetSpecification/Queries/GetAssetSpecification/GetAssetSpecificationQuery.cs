using Core.Application.Common.HttpResponse;
using Core.Domain.Entities.AssetMaster;
using MediatR;

namespace Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification
{
    public class GetAssetSpecificationQuery : IRequest<ApiResponseDTO<List<AssetSpecificationDTO>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; } 
        public string? SearchTerm { get; set; }
    }

}