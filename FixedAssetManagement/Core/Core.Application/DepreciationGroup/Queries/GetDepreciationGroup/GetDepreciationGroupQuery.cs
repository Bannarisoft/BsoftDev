using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.DepreciationGroup.Queries.GetDepreciationGroup
{
    public class GetDepreciationGroupQuery : IRequest<(List<DepreciationGroupDTO>,int)>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; } 
        public string? SearchTerm { get; set; }
    }
}