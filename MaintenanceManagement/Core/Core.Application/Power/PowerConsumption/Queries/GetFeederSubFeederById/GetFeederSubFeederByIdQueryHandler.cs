using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.Power.IPowerConsumption;
using Core.Application.Power.PowerConsumption.Queries.GetFeederSubFeederById;
using Core.Domain.Events;
using MassTransit.Futures.Contracts;
using MediatR;

namespace Core.Application.Power.PowerConsumption.Queries
{
    public class GetFeederSubFeederByIdQueryHandler : IRequestHandler<GetFeederSubFeederByIdQuery, List<GetFeederSubFeederDto>>
    {
        private readonly IPowerConsumptionQueryRepository _powerConsumptionQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetFeederSubFeederByIdQueryHandler(IPowerConsumptionQueryRepository powerConsumptionQueryRepository, IMapper mapper, IMediator mediator)
        {
            _powerConsumptionQueryRepository = powerConsumptionQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<List<GetFeederSubFeederDto>> Handle(GetFeederSubFeederByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _powerConsumptionQueryRepository.GetFeederSubFeedersById(request.FeederTypeId);
          
            var FeederSubFeeder = _mapper.Map<List<GetFeederSubFeederDto>>(result);
            //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetFeederSubFeederByIdQuery",
                    actionCode: "GetFeederSubFeederByIdQuery",        
                    actionName: "Feeders & SubFeeders Load", 
                    details: $"Feeder & SubFeeder details was fetched.",
                    module:"Feeder & SubFeeder"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return  FeederSubFeeder;
        }
    }
}