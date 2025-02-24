

using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetSpecification.Commands.CreateAssetSpecification
{
    public class CreateAssetSpecificationCommand : IRequest<ApiResponseDTO<AssetSpecificationDTO>>  
    {
        public int AssetId { get; set; }
        public int ManufactureId { get; set; }
         public DateTimeOffset? ManufactureDate { get; set; } 
        public int SpecificationId { get; set; }
        public string? SpecificationValue { get; set; }
        public string? SerialNumber { get; set; }
        public string? ModelNumber { get; set; }
    }
}