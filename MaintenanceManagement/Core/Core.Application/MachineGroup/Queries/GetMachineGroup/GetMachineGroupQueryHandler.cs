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

namespace Core.Application.MachineGroup.Queries.GetMachineGroup
{
    public class GetMachineGroupQueryHandler : IRequestHandler<GetMachineGroupQuery, ApiResponseDTO<List<MachineGroupDto>>>
    {
       private readonly IMachineGroupQueryRepository _machineGroupQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IDepartmentGrpcClient _departmentGrpcClient;


        public GetMachineGroupQueryHandler(IMachineGroupQueryRepository machineGroupQueryRepository, IMapper mapper, IMediator mediator, IDepartmentGrpcClient departmentGrpcClient)
        {
            _machineGroupQueryRepository = machineGroupQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
            _departmentGrpcClient = departmentGrpcClient;
        }
         public async Task<ApiResponseDTO<List<MachineGroupDto>>> Handle(GetMachineGroupQuery request, CancellationToken cancellationToken)
        {
            // Fetch data from repository
            var (machineGroups, totalCount) = await _machineGroupQueryRepository.GetAllMachineGroupsAsync(request.PageNumber, request.PageSize, request.SearchTerm);

            // Map domain entities to DTOs
            var machineGroupList = _mapper.Map<List<MachineGroupDto>>(machineGroups);
              // 🔥 Fetch lookups
            var departments = await _departmentGrpcClient.GetAllDepartmentAsync();
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);
                 // 🔁 Set DepartmentName and UnitName in one loop
            foreach (var dto in machineGroupList)
            {
                if (departmentLookup.TryGetValue(dto.DepartmentId, out var deptName))
                    dto.DepartmentName = deptName;

              
            }

            // Publish domain event for auditing
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "",
                actionName: "",
                details: "MachineGroup details were fetched.",
                module: "MachineGroup"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            // Return API response
            return new ApiResponseDTO<List<MachineGroupDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = machineGroupList,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}