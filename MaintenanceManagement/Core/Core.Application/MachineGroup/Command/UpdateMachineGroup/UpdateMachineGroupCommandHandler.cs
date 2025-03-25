using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineGroup.Command.UpdateMachineGroup
{
    public class UpdateMachineGroupCommandHandler : IRequestHandler<UpdateMachineGroupCommand, ApiResponseDTO<bool>>
    {
          private readonly IMachineGroupCommandRepository _machineGroupCommandRepository;
        private readonly IMachineGroupQueryRepository _machineGroupQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;


        public UpdateMachineGroupCommandHandler(
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


         public async Task<ApiResponseDTO<bool>> Handle(UpdateMachineGroupCommand request, CancellationToken cancellationToken)
        {
         
            // Map request to domain entity
            var machineGroup = _mapper.Map<Core.Domain.Entities.MachineGroup>(request);

            // Update the MachineGroup entity in the database
            var updateResult = await _machineGroupCommandRepository.UpdateAsync(request.Id, machineGroup);

            // Publish domain event for logging/audit purposes
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: machineGroup.GroupName,
                actionName: machineGroup.Manufacturer.ToString(),
                details: $"MachineGroup '{machineGroup.Id}' was updated.",
                module: "MachineGroup"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            // Return appropriate response based on update result
            if (updateResult)
            {
                return new ApiResponseDTO<bool> { IsSuccess = true, Message = "MachineGroup updated successfully." };
            }

            return new ApiResponseDTO<bool> { IsSuccess = false, Message = "MachineGroup update failed." };
        }

    }
}