using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Manufacture.Queries.GetManufacture
{
    public class GetManufactureQuery : IRequest<ApiResponseDTO<List<ManufactureDTO>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; } 
        public string? SearchTerm { get; set; }
    }
}