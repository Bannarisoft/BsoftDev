using Core.Application.Common.HttpResponse;
using Core.Application.SpecificationMaster.Queries.GetSpecificationMaster;
using MediatR;

namespace Core.Application.SpecificationMaster.Commands.DeleteSpecificationMaster
{
    public class DeleteSpecificationMasterCommand :  IRequest<ApiResponseDTO<SpecificationMasterDTO>>  
    {
         public int Id { get; set; }    
    }
}