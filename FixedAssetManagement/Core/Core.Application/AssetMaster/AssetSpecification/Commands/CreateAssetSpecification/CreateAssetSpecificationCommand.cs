

using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetSpecification.Commands.CreateAssetSpecification
{
   public class CreateAssetSpecificationCommand : IRequest<ApiResponseDTO<string>>
    {
        public int AssetId { get; set; }
        public List<SpecificationItem>? Specifications { get; set; }
    }

    public class SpecificationItem
    {
        public int SpecificationId { get; set; }
        public string? SpecificationName { get; set; }
        public string? SpecificationValue { get; set; }        
    }
}