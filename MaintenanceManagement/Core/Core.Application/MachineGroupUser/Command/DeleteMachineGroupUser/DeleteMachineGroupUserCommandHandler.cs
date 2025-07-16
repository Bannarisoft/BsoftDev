using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineGroupUser;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineGroupUser.Command.DeleteMachineGroupUser
{
    public class DeleteMachineGroupUserCommandHandler  : IRequestHandler<DeleteMachineGroupUserCommand, bool>
    {
        private readonly IMachineGroupUserCommandRepository _machineGroupUserCommand;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public DeleteMachineGroupUserCommandHandler(IMachineGroupUserCommandRepository machineGroupUserCommand, IMapper mapper, IMediator mediator)
        {
            _machineGroupUserCommand = machineGroupUserCommand;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<bool> Handle(DeleteMachineGroupUserCommand request, CancellationToken cancellationToken)
        {
                var machineGroupUser  = _mapper.Map<Core.Domain.Entities.MachineGroupUser>(request);
                var machineGroupUserResult = await _machineGroupUserCommand.DeleteAsync(request.Id,machineGroupUser);

                //Domain Event  
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Delete",
                    actionCode: "delete",
                    actionName: "Delete MachineGroup User",
                    details: $"Delete MachineGroup User",
                    module:"MachineGroupUser"
                );               
                await _mediator.Publish(domainEvent, cancellationToken);  

             return machineGroupUserResult == true ? machineGroupUserResult : throw new ExceptionRules("MachineGroupUser deletion Failed.");
                
                
        }
    }
}