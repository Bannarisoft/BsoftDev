using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces.IPartyGroup;
using Core.Application.PartyGroup.Queries.GetPartyGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.PartyGroup.Queries.GetPartyGroupById
{
    public class GetPartyGroupByIdQueryHandler : IRequestHandler<GetPartyGroupByIdQuery, PartyGroupByIdDto>
    {
        private readonly IPartyGroupQueryRepository _ipartygroupQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetPartyGroupByIdQueryHandler(IPartyGroupQueryRepository ipartygroupQueryRepository, IMapper mapper, IMediator mediator)
        {
            _ipartygroupQueryRepository = ipartygroupQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }       

        public async Task<PartyGroupByIdDto> Handle(GetPartyGroupByIdQuery request, CancellationToken cancellationToken)
        {
             var result = await _ipartygroupQueryRepository.GetByIdAsync(request.Id);
          
            // Map a single entity
            var partymaster = _mapper.Map<PartyGroupByIdDto>(result);
       
          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "GetPartyGroupByIdQuery",        
                    actionName: partymaster.Id.ToString(),
                    details: $"PartyGroup details {partymaster.Id} was fetched.",
                    module:"PartyGroup"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return partymaster;
        }
    }
}