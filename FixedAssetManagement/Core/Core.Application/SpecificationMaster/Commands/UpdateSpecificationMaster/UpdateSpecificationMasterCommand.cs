using Core.Application.Common.HttpResponse;
using Core.Application.SpecificationMaster.Queries.GetSpecificationMaster;
using MediatR;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.SpecificationMaster.Commands.UpdateSpecificationMaster
{
    public class UpdateSpecificationMasterCommand  : IRequest<ApiResponseDTO<SpecificationMasterDTO>> 
    {
        public int Id { get; set; }       
        public string? SpecificationName { get; set; }
        public int? AssetGroupId { get; set; }
        public bool IsDefault { get; set; }               
        public Status IsActive { get; set; }
    }
}