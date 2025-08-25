using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.Item.PutAway;
using Core.Application.Item.PutAway.Queries.GetAllPutAwayRule;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Item.PutAway.Queries.EvaluatePutAway
{
  /*   public class EvaluatePutAwayQueryHandler : IRequestHandler<EvaluatePutAwayQuery, ApiResponseDTO<PutAwayEvaluateResult>>
    {
        private readonly IPutAwayRuleQueryRepository _queryRepo;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public EvaluatePutAwayQueryHandler(IPutAwayRuleQueryRepository queryRepo, IMediator mediator, IMapper mapper)
        {
            _queryRepo = queryRepo;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<ApiResponseDTO<PutAwayEvaluateResult>> Handle(EvaluatePutAwayQuery request, CancellationToken cancellationToken)
        {
            var result = await _queryRepo.EvaluateAsync(request.Body, cancellationToken);

            // 📘 Audit
            var ev = new AuditLogsDomainEvent(
                actionDetail: "Evaluate",
                actionCode: result is null ? "Evaluate:NoCandidate" : "Evaluate:Success",
                actionName: result is null ? "0" : "1",
                details: result is null ? "No suitable location found." : $"Chosen: {result.LocationCode}",
                module: "PutAway"
            );
            await _mediator.Publish(ev, cancellationToken);

            return new ApiResponseDTO<PutAwayEvaluateResult>
            {
                IsSuccess = result is not null,
                Message = result is null ? "No matching rule or no suitable location found." : "Success",
                Data = result
            };
        }
    } */
}
