using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineGroup.Queries.GetMachineGroupById
{
    public class GetMachineGroupByIdQueryHandler  : IRequestHandler<GetMachineGroupByIdQuery, ApiResponseDTO<GetMachineGroupByIdDto>>
    {

        private readonly IMachineGroupQueryRepository  _machineGroupQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IDepartmentGrpcClient _departmentGrpcClient;

        public GetMachineGroupByIdQueryHandler(IMachineGroupQueryRepository machineGroupQueryRepository, IMapper mapper, IMediator mediator, IDepartmentGrpcClient departmentGrpcClient)
        {
            _machineGroupQueryRepository = machineGroupQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
            _departmentGrpcClient = departmentGrpcClient;
        } 

         public async Task<ApiResponseDTO<GetMachineGroupByIdDto>> Handle(GetMachineGroupByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _machineGroupQueryRepository.GetByIdAsync(request.Id);
            
            if (result is null)
            {
                return new ApiResponseDTO<GetMachineGroupByIdDto>
                {
                    IsSuccess = false,
                    Message = $"MachineGroup with Id {request.Id} not found.",
                    Data = null
                };
            }
            
            var machineGroup = _mapper.Map<GetMachineGroupByIdDto>(result);
              // 🔥 Fetch lookups
            var departments = await _departmentGrpcClient.GetAllDepartmentAsync();
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);
         if (departmentLookup.TryGetValue(machineGroup.DepartmentId, out var departmentName))
            {
                machineGroup.DepartmentName = departmentName!;
            }

            // Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: "",
                actionName: "",
                details: $"MachineGroup details {machineGroup.Id} were fetched.",
                module: "MachineGroup"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<GetMachineGroupByIdDto>
            {
                IsSuccess = true,
                Message = "Success",
                Data = machineGroup
            };
        }



    }
}