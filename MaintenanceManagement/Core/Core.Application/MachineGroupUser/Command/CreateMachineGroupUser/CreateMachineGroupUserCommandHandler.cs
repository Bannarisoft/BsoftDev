using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineGroupUser;
using MediatR;
using Core.Domain.Entities;
using Core.Domain.Events;

namespace Core.Application.MachineGroupUsers.Command.CreateMachineGroupUser
{
    public class CreateMachineGroupUsersCommandHandler  : IRequestHandler<CreateMachineGroupUserCommand, ApiResponseDTO<int>>
    {
        private readonly IMachineGroupUserCommandRepository _machineGroupUsersCommand;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public CreateMachineGroupUsersCommandHandler(IMachineGroupUserCommandRepository machineGroupUsersCommand, IMediator mediator, IMapper mapper)
        {
            _machineGroupUsersCommand = machineGroupUsersCommand;
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task<ApiResponseDTO<int>> Handle(CreateMachineGroupUserCommand request, CancellationToken cancellationToken)
        {
                var machineGroupUsers  = _mapper.Map<Core.Domain.Entities.MachineGroupUser>(request);

                var machineGroupUsersResult = await _machineGroupUsersCommand.CreateAsync(machineGroupUsers);                
                
                if (machineGroupUsersResult > 0)
                {
                    var domainEvent = new AuditLogsDomainEvent(
                     actionDetail: "Create",
                     actionCode: "NewData",
                     actionName: "MachineGroup Users Creation",
                     details: $"MachineGroup Users Creation",
                     module:"MachineGroupUsers Master"
                 );
                 await _mediator.Publish(domainEvent, cancellationToken);
                 
                    return new ApiResponseDTO<int>
                    {
                        IsSuccess = true, 
                        Message = "MachineGroup Users successfully created", 
                        Data = machineGroupUsersResult
                    };
                }
               
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false, 
                    Message = "MachineGroup Users not created"
                };
        }
    }
}