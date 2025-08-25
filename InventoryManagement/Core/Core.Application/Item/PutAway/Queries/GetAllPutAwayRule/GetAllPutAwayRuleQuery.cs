using Core.Application.Common.HttpResponse;
using Core.Application.Item.PutAway.Queries.GetAllPutAwayRule;
using MediatR;

namespace Core.Application.Item.PutAway.Queries.GetPutAwayRules
{
    public class GetPutAwayRulesQuery : IRequest<ApiResponseDTO<List<PutAwayRuleListDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize  { get; set; } = 15;
        public string? SearchTerm { get; set; }
    }
}
