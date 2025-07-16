using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecificationById
{
    public class GetAssetSpecificationByIdQuery : IRequest<ApiResponseDTO<AssetSpecificationJsonDto>>
    {
         public int Id { get; set; }
    }
}