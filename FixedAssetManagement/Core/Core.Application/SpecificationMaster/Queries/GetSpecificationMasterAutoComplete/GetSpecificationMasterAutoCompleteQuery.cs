using Core.Application.Common.HttpResponse;
using Core.Application.SpecificationMaster.Queries.GetSpecificationMaster;
using MediatR;

namespace Core.Application.SpecificationMaster.Queries.GetSpecificationMasterAutoComplete
{
    public class GetSpecificationMasterAutoCompleteQuery : IRequest<ApiResponseDTO<List<SpecificationMasterAutoCompleteDTO>>>    
    {
         public string? SearchPattern { get; set; }
    }
}