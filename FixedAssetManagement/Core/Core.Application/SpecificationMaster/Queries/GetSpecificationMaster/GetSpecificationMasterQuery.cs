using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.SpecificationMaster.Queries.GetSpecificationMaster
{
    public class GetSpecificationMasterQuery : IRequest<ApiResponseDTO<List<SpecificationMasterDTO>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; } 
        public string? SearchTerm { get; set; }
    }
}