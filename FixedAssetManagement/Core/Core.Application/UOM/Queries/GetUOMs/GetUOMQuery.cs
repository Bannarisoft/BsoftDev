using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.UOM.Queries.GetUOMs
{
    public class GetUOMQuery : IRequest<ApiResponseDTO<List<UOMDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public string? SearchTerm { get; set; }
    }
}