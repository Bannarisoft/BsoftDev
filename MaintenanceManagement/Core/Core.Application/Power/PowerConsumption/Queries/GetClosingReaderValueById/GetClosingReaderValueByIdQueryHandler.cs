using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.Power.IPowerConsumption;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Power.PowerConsumption.Queries.GetClosingReaderValueById
{
    public class GetClosingReaderValueByIdQueryHandler : IRequestHandler<GetClosingReaderValueByIdQuery, GetClosingReaderValueDto>
    {
        private readonly IPowerConsumptionQueryRepository _powerConsumptionQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetClosingReaderValueByIdQueryHandler(IPowerConsumptionQueryRepository powerConsumptionQueryRepository, IMapper mapper, IMediator mediator)
        {
            _powerConsumptionQueryRepository = powerConsumptionQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<GetClosingReaderValueDto> Handle(GetClosingReaderValueByIdQuery request, CancellationToken cancellationToken)
        {
             var result = await _powerConsumptionQueryRepository.GetOpeningReaderValueById(request.FeederId);


            // Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetClosingReaderValueByIdQuery",
                actionCode: "GetClosingReaderValueByIdQuery",
                actionName: "Closing Reader Value Load",
                details: "Closing Reader Value details was fetched.",
                module: "Power"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            return  result;
        }
    }
}