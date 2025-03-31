

using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineGroupUser;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineGroupUser.Command.UpdateMachineGroupUser
{
    public class UpdateMachineGroupUserCommandHandler  : IRequestHandler<UpdateMachineGroupUserCommand, ApiResponseDTO<bool>>
    {
        private readonly IMachineGroupUserCommandRepository _machineGroupUserCommand;
         private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public UpdateMachineGroupUserCommandHandler(IMachineGroupUserCommandRepository machineGroupUserCommand, IMapper mapper, IMediator mediator)
        {
            _machineGroupUserCommand = machineGroupUserCommand;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<bool>> Handle(UpdateMachineGroupUserCommand request, CancellationToken cancellationToken)
        {
            var machineGroupUser  = _mapper.Map<Core.Domain.Entities.MachineGroupUser>(request);
         
            var machineGroupUserResult = await _machineGroupUserCommand.UpdateAsync(machineGroupUser);

            
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: "update",
                    actionName: "Update MachineGroup User ",
                    details: $"Update MachineGroup User ",
                    module:"MachineGroup User "
                );               
                await _mediator.Publish(domainEvent, cancellationToken); 
            
            if(machineGroupUserResult)
            {
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = true, 
                    Message = "MachineGroup User updated successfully."
                };
            }

            return new ApiResponseDTO<bool>
            {
                IsSuccess = false, 
                Message = "MachineGroup User  not updated."
            };
        }
    }
}