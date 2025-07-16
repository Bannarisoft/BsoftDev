using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineGroup.Queries.GetMachineGroupAutoComplete
{
    public class GetMachineGroupAutoCompleteQueryHandler : IRequestHandler<GetMachineGroupAutoCompleteQuery,List<GetMachineGroupAutoCompleteDto>>
    {

        private readonly IMachineGroupQueryRepository  _machineGroupQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;


         public GetMachineGroupAutoCompleteQueryHandler(IMachineGroupQueryRepository machineGroupQueryRepository , IMapper mapper, IMediator mediator)
         {
           
            _machineGroupQueryRepository = machineGroupQueryRepository;
            _mapper =mapper;
            _mediator = mediator;
         }

           public  async Task<List<GetMachineGroupAutoCompleteDto>> Handle(GetMachineGroupAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var machineGroup  = await _machineGroupQueryRepository.GetMachineGroupAutoComplete(request.SearchPattern);


            var machine = _mapper.Map<List<GetMachineGroupAutoCompleteDto>>(machineGroup);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",        
                    actionName: "", 
                    details: $"MachineGroup details was fetched.",
                    module:"MachineGroup"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return machine;
        }

        
    }
}