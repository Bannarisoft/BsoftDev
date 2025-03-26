using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineGroup;
using Core.Application.MachineGroup.Quries.GetMachineGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineGroup.Command.CreateMachineGroup
{
    public class CreateMachineGroupCommandHandler : IRequestHandler<CreateMachineGroupCommand, ApiResponseDTO<MachineGroupDto>>
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

        public async Task<ApiResponseDTO<MachineGroupDto>> Handle(CreateMachineGroupCommand request, CancellationToken cancellationToken)
        {
           
            // 🔹 Map request to domain entity
            var machineGroup = _mapper.Map<Core.Domain.Entities.MachineGroup>(request);

            // 🔹 Insert into the database
            var result = await _machineGroupCommandRepository.CreateAsync(machineGroup);

            if (result.Id <= 0)
            {
                return new ApiResponseDTO<MachineGroupDto>
                {
                    IsSuccess = false,
                    Message = "Failed to create Machine Group",
                    Data = null
                };
            }

            // 🔹 Fetch newly created record
            var createdMachineGroup = await _machineGroupQueryRepository.GetByIdAsync(result.Id);
            var mappedResult = _mapper.Map<MachineGroupDto>(createdMachineGroup);

            // 🔹 Publish domain event for auditing/logging
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: machineGroup.GroupName,
                actionName: "Machine Group Created",
                details: $"Machine Group '{machineGroup.GroupName}' was created.",
                module: "MachineGroup"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            // 🔹 Return success response
            return new ApiResponseDTO<MachineGroupDto>
            {
                IsSuccess = true,
                Message = "Machine Group created successfully",
                Data = mappedResult
            };
        }
       
    }
}