using Core.Application.Common.HttpResponse;
using Core.Application.SpecificationMaster.Queries.GetSpecificationMaster;
using MediatR;

namespace Core.Application.SpecificationMaster.Commands.CreateSpecificationMaster
{
    public class CreateSpecificationMasterCommand : IRequest<ApiResponseDTO<SpecificationMasterDTO>>  
    {
        public string? SpecificationName { get; set; }      
        public int? AssetGroupId { get; set; } 
        public bool? ISDefault { get; set; }       

    }
}