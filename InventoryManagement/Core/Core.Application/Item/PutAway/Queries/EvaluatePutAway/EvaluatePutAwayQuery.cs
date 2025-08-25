using Core.Application.Common.HttpResponse;
using Core.Application.Item.PutAway.Queries.GetAllPutAwayRule;
using MediatR;

namespace Core.Application.Item.PutAway.Queries.EvaluatePutAway
{
    public class EvaluatePutAwayQuery : IRequest<ApiResponseDTO<PutAwayEvaluateResult>>
    {
        public PutAwayEvaluateRequest Body { get; set; } = default!;
    }
}
