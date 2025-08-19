using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IRackMaster;
using Core.Application.RackMaster.Queries.GetAllRackMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.RackMaster.Queries.GetRackMasterById
{
    public class GetRackMasterByIdQueryHandler : IRequestHandler<GetRackMasterByIdQuery, RackMasterDto>
    {

        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        private readonly IRackMasterQueryRepository _rackMasterQueryRepository;


        public GetRackMasterByIdQueryHandler(IMapper mapper, IRackMasterQueryRepository rackMasterQueryRepository, IMediator mediator)
        {
            _mapper = mapper;
            _rackMasterQueryRepository = rackMasterQueryRepository;
            _mediator = mediator;
        }

        public async Task<RackMasterDto> Handle(GetRackMasterByIdQuery request, CancellationToken cancellationToken)
        {

            var result = await _rackMasterQueryRepository.GetByIdAsync(request.Id);
            if (result is null )
            {
                throw new ValidationException($"MiscTypeMaster with Id {request.Id} not found.");
              
            }
           
            var misctypemaster = _mapper.Map<RackMasterDto>(result);

            //Domain Event
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "GetById",
                        actionCode: "",        
                        actionName: "",
                        details: $"MiscTypeMaster details {misctypemaster.Id} was fetched.",
                        module:"MiscTypeMaster"
                    );
                    await _mediator.Publish(domainEvent, cancellationToken);
            return  misctypemaster;
        }
           
        }
    
}