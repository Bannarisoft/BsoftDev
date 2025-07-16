using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.Power.IFeederGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Power.FeederGroup.Queries.GetFeederGroupAutoComplete
{
    public class GetFeederGroupAutoCompleteQueryHandler : IRequestHandler<GetFeederGroupAutoCompleteQuery, List<GetFeederGroupAutoCompleteDto>>
    {
        private readonly IFeederGroupQueryRepository _feederGroupQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetFeederGroupAutoCompleteQueryHandler(IFeederGroupQueryRepository feederGroupQueryRepository, IMapper mapper, IMediator mediator)
        {
            _feederGroupQueryRepository = feederGroupQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
          public  async Task<List<GetFeederGroupAutoCompleteDto>> Handle(GetFeederGroupAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var machineGroup  = await _feederGroupQueryRepository.GetFeederGroupAutoComplete(request.SearchPattern);

            var division = _mapper.Map<List<GetFeederGroupAutoCompleteDto>>(machineGroup);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",        
                    actionName: "", 
                    details: $"FeederGroup details was fetched.",
                    module:"FeederGroup"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return  division; 
        }


    }
}