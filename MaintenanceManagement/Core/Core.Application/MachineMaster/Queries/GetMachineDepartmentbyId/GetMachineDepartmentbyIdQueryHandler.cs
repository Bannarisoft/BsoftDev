using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineMaster.Queries.GetMachineDepartmentbyId
{
    public class GetMachineDepartmentbyIdQueryHandler : IRequestHandler<GetMachineDepartmentbyIdQuery, ApiResponseDTO<MachineDepartmentGroupDto>>
    {
        private readonly IMachineMasterQueryRepository _imachineMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetMachineDepartmentbyIdQueryHandler(IMachineMasterQueryRepository imachineMasterQueryRepository, IMapper mapper, IMediator mediator)
        {
            _imachineMasterQueryRepository = imachineMasterQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<MachineDepartmentGroupDto>> Handle(GetMachineDepartmentbyIdQuery request, CancellationToken cancellationToken)
        {
             var result = await _imachineMasterQueryRepository.GetMachineDepartment(request.MachineGroupId);
            // Check if the entity exists
            if (result is null)
            {
                return new ApiResponseDTO<MachineDepartmentGroupDto> { IsSuccess = false, Message =$"Machine ID {request.MachineGroupId} not found." };
            }
            // Map a single entity
            var machineMaster = _mapper.Map<MachineDepartmentGroupDto>(result);
            //var departments = await _departmentGrpcClient.GetAllDepartmentsAsync();
                // var dept = departments.FirstOrDefault(d => d.DepartmentId == machineMaster.DepartmentId);
                // if (dept != null)
                // {
                //     machineMaster.DepartmentName = dept.DepartmentName;
                // }
                // else
                // {
                //     // Optional: handle missing department
                //     machineMaster.DepartmentName = "Unknown";
                // }
          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "GetMachineDepartmentbyIdQuery",        
                    actionName: machineMaster.DepartmentName.ToString(),
                    details: $"Machine department details {machineMaster.DepartmentGroupName} was fetched.",
                    module:"MachineDepartment"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return new ApiResponseDTO<MachineDepartmentGroupDto> { IsSuccess = true, Message = "Success", Data = machineMaster };
        }
    }
}