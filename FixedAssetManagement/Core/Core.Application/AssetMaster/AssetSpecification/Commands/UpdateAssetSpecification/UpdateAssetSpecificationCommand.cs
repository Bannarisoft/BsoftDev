using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification;
using Core.Application.Common.HttpResponse;
using MediatR;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.AssetMaster.AssetSpecification.Commands.UpdateAssetSpecification
{
    public class UpdateAssetSpecificationCommand : IRequest<ApiResponseDTO<AssetSpecificationJsonDto>> 
    {
        public int Id { get; set; }      
        public int AssetId { get; set; }
        public int ManufactureId { get; set; }
        public DateTimeOffset? ManufactureDate { get; set; } 
        public int SpecificationId { get; set; }
        public string? SpecificationValue { get; set; }
        public string? SerialNumber { get; set; }
        public string? ModelNumber { get; set; }
        public Status IsActive { get; set; }
    }
}