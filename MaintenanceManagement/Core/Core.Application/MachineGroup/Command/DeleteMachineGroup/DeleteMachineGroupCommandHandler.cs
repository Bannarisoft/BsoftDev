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

namespace Core.Application.MachineGroup.Command.DeleteMachineGroup
{
    public class DeleteMachineGroupCommandHandler  : IRequestHandler<DeleteMachineGroupCommand, bool>
    {
        
         private readonly IMachineGroupCommandRepository  _machineGroupRepository;
        private readonly IMapper _imapper;
        private readonly IMediator _mediator;


         public DeleteMachineGroupCommandHandler(IMachineGroupCommandRepository machineGroupRepository, IMapper imapper, IMediator mediator)
        {
            _machineGroupRepository = machineGroupRepository;
            _imapper = imapper;
            _mediator = mediator;
        }

        public async Task<bool> Handle(DeleteMachineGroupCommand request, CancellationToken cancellationToken)
        {

             // Map the request to the entity
            var machineGroup = _imapper.Map<Core.Domain.Entities.MachineGroup>(request);

            // Perform the delete operation
            var isDeleted = await _machineGroupRepository.DeleteAsync(request.Id, machineGroup);

            // Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: machineGroup.Id.ToString(),
                actionName: machineGroup.IsDeleted.ToString(),
                details: $"MachineGroup with ID {machineGroup.Id} was deleted.",
                module: "MachineGroup"
            );
            await _mediator.Publish(domainEvent, cancellationToken);


            return isDeleted==true ? isDeleted : throw new ExceptionRules("Machine Group deletion Failed.");
        }
    }
}