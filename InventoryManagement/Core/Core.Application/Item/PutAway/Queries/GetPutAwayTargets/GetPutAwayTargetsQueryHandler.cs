using AutoMapper;
using Core.Application.Common.Interfaces.Item.PutAway;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Item.PutAway.Queries.GetPutAwayTargets
{
    //public class GetPutAwayTargetsQueryHandler : IRequestHandler<GetPutAwayTargetsQuery, List<PutAwayTargetLookupDto>>
    //{
       /*  private readonly IPutAwayRuleQueryRepository _putAwayQueryRepo;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GetPutAwayTargetsQueryHandler(
            IPutAwayRuleQueryRepository putAwayQueryRepo,
            IMediator mediator,
            IMapper mapper)
        {
            _putAwayQueryRepo = putAwayQueryRepo;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<List<PutAwayTargetLookupDto>> Handle(GetPutAwayTargetsQuery request, CancellationToken cancellationToken)
        {
            // Fetch targets by Misc-based storage type
            var rows = await _putAwayQueryRepo.GetTargetsByMiscAsync(
                warehouseId: request.WarehouseId,
                storageTypeMiscId: request.StorageTypeId,
                searchPattern: request.SearchPattern,
                ct: cancellationToken);

            // If your repo already returns PutAwayTargetLookupDto, mapping is a no-op; keep for consistency
            var dto = _mapper.Map<List<PutAwayTargetLookupDto>>(rows);

            // Audit log
            var ev = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "GetPutAwayTargetsByMisc",
                actionName: dto.Count.ToString(),
                details: $"Targets fetched (WarehouseId={request.WarehouseId}, StorageTypeId={request.StorageTypeId}, Search='{request.SearchPattern ?? ""}')",
                module: "PutAway");
            await _mediator.Publish(ev, cancellationToken);

            return dto;
        } */
//    }
}
