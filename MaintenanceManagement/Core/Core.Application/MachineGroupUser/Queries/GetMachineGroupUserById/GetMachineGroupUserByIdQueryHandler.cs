
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineGroupUser;
using Core.Application.MachineGroupUser.Queries.GetMachineGroupUser;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineGroupUser.Queries.GetMachineGroupUserById
{
    public class GetMachineGroupUserByIdQueryHandler : IRequestHandler<GetMachineGroupUserByIdQuery, ApiResponseDTO<MachineGroupUserDto>>
    {
        private readonly IMachineGroupUserQueryRepository _machineGroupQuery;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IDepartmentAllGrpcClient _departmentAllGrpcClient;

        public GetMachineGroupUserByIdQueryHandler(IMachineGroupUserQueryRepository machineGroupQuery, IMapper mapper, IMediator mediator, IDepartmentAllGrpcClient departmentAllGrpcClient)
        {
            _machineGroupQuery = machineGroupQuery;
            _mapper = mapper;
            _mediator = mediator;
            _departmentAllGrpcClient = departmentAllGrpcClient;
        }
        public async Task<ApiResponseDTO<MachineGroupUserDto>> Handle(GetMachineGroupUserByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _machineGroupQuery.GetByIdAsync(request.Id);
            var machineGroupResult = _mapper.Map<MachineGroupUserDto>(result);
           
             // 🔥 Fetch departments using gRPC
            var departments = await _departmentAllGrpcClient.GetDepartmentAllAsync();
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

            // 🔥 Map department name
            if (departmentLookup.TryGetValue(machineGroupResult.DepartmentId, out var departmentName) && departmentName != null)
            {
                machineGroupResult.DepartmentName = departmentName;
            }
            
          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "",        
                    actionName: "",
                    details: $"MachineGroup User details was fetched.",
                    module:"MachineGroup User "
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return new ApiResponseDTO<MachineGroupUserDto> 
          { 
            IsSuccess = true, 
            Message = "Success", 
            Data = machineGroupResult 
            };
        }
    }
}