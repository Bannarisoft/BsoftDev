using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces.IPartyMaster;
using Core.Domain.Events;
using MediatR;
using static Core.Application.PartyMaster.Queries.GetPartyMasterById.PartyMasterDto;

namespace Core.Application.PartyMaster.Queries.GetPartyMasterById
{
    public class GetPartyMasterByIdQueryHandler : IRequestHandler<GetPartyMasterByIdQuery, PartyMasterDto>
    {
        private readonly IPartyMasterQueryRepository _ipartyMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetPartyMasterByIdQueryHandler(IPartyMasterQueryRepository ipartyMasterQueryRepository, IMapper mapper, IMediator mediator)
        {
            _ipartyMasterQueryRepository = ipartyMasterQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<PartyMasterDto> Handle(GetPartyMasterByIdQuery request, CancellationToken cancellationToken)
        {
           var dto = await _ipartyMasterQueryRepository.GetByIdPartyMasterAsync(request.PartyId);

            if (dto == null)
                throw new KeyNotFoundException("PartyId not found");

            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: "GetPartyMasterByIdQuery",
                actionName: dto.Id.ToString(),
                details: $"Party details {dto.Id} fetched.",
                module: "PartyMaster"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            return dto;
        }
    }
}