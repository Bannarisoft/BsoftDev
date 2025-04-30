using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IActivityMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineGroup.Queries.GetMachineGroupById
{
    public class GetActivityMasterByIdQueryHandler  : IRequestHandler<GetActivityMasterByIdQuery, ApiResponseDTO<GetActivityMasterByIdDto>>
    {

        private readonly IActivityMasterQueryRepository _activityMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IDepartmentGrpcClient _departmentGrpcClient; // ✅ Interface, not DepartmentServiceClient

        

         public GetActivityMasterByIdQueryHandler(IActivityMasterQueryRepository activityMasterQueryRepository, IMapper mapper, IMediator mediator, IDepartmentGrpcClient departmentGrpcClient)
        {
            _activityMasterQueryRepository = activityMasterQueryRepository;
            _mapper =mapper;
            _mediator = mediator;
            _departmentGrpcClient = departmentGrpcClient;

        } 

         public async Task<ApiResponseDTO<GetActivityMasterByIdDto>> Handle(GetActivityMasterByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _activityMasterQueryRepository.GetByIdAsync(request.Id);
            
            if (result is null)
            {
                return new ApiResponseDTO<GetActivityMasterByIdDto>
                {
                    IsSuccess = false,
                    Message = $"ActivityMaster with Id {request.Id} not found.",
                    Data = null
                };
            }
            
            var machineGroup = _mapper.Map<GetActivityMasterByIdDto>(result);

             // 🔥 Fetch departments using gRPC
            var departments = await _departmentGrpcClient.GetAllDepartmentsAsync();
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

            var activityMasterDictionary = new Dictionary<int, GetActivityMasterByIdDto>();

           // ✅ No foreach needed
                    if (departmentLookup.TryGetValue(machineGroup.DepartmentId, out var departmentName) && departmentName != null)
                    {
                        machineGroup.Department = departmentName;
                    }

            // Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: "",
                actionName: "",
                details: $"ActivityMaster details {machineGroup.Id} were fetched.",
                module: "ActivityMaster"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<GetActivityMasterByIdDto>
            {
                IsSuccess = true,
                Message = "Success",
                Data = machineGroup
            };
        }



    }
}