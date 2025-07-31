
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Item.ItemGroup.Queries.GetItemGroup
{
    public class GetItemGroupQuery : IRequest<ApiResponseDTO<List<ItemGroupDto>>>
    {        
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public string? SearchTerm { get; set; }
    }
}