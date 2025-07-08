using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineGroup;
using Core.Application.MachineGroup.Queries.GetMachineGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineGroup.Command.CreateMachineGroup
{
    public class CreateMachineGroupCommandHandler : IRequestHandler<CreateMachineGroupCommand, int>
    {

         private readonly IMachineGroupCommandRepository _machineGroupCommandRepository;
        private readonly IMachineGroupQueryRepository _machineGroupQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public CreateMachineGroupCommandHandler(
            IMachineGroupCommandRepository machineGroupCommandRepository,
            IMachineGroupQueryRepository machineGroupQueryRepository,
            IMapper mapper,
            IMediator mediator)
        {
            _machineGroupCommandRepository = machineGroupCommandRepository;
            _machineGroupQueryRepository = machineGroupQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<int> Handle(CreateMachineGroupCommand request, CancellationToken cancellationToken)
        {
           
            
            var machineGroup = _mapper.Map<Core.Domain.Entities.MachineGroup>(request);

            
            var result = await _machineGroupCommandRepository.CreateAsync(machineGroup);

            
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: machineGroup.GroupName,
                actionName: "Machine Group Created",
                details: $"Machine Group '{machineGroup.GroupName}' was created.",
                module: "MachineGroup"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            // 🔹 Return success response
            return result.Id > 0 ? result.Id : throw new ExceptionRules("Machine Group Creation Failed.");
        }
       
    }
}