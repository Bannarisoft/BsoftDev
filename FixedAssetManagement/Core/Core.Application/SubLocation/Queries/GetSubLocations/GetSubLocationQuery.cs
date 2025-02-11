using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.SubLocation.Queries.GetSubLocations
{
    public class GetSubLocationQuery : IRequest<ApiResponseDTO<List<SubLocationDto>>>
    {
        
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public string? SearchTerm { get; set; }
    }
}