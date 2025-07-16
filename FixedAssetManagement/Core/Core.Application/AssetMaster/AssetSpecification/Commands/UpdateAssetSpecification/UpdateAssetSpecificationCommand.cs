using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification;
using Core.Application.Common.HttpResponse;
using MediatR;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.AssetMaster.AssetSpecification.Commands.UpdateAssetSpecification
{
     public class UpdateAssetSpecificationCommand : IRequest<ApiResponseDTO<string>>
    {
        public int AssetId { get; set; }
        public List<UpdateSpecificationItem>? Specifications { get; set; }
    }

    public class UpdateSpecificationItem
    {
        public int SpecificationId { get; set; }
        public string? SpecificationValue { get; set; }
        public byte IsActive  { get; set; }
    }
}