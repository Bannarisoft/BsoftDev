
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.WorkOrder.Queries.GetWorkOrderById;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WorkOrder.Queries.GetWorkOrder
{
    public class GetWorkOrderQueryHandler : IRequestHandler<GetWorkOrderQuery, ApiResponseDTO<List<Dictionary<string, List<GetWorkOrderDto>>>>>
    {
        private readonly IWorkOrderQueryRepository _workOrderRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        private readonly IDepartmentGrpcClient _departmentGrpcClient;

        public GetWorkOrderQueryHandler(IWorkOrderQueryRepository workOrderRepository , IMapper mapper, IMediator mediator, IDepartmentGrpcClient departmentGrpcClient)
        {
            _workOrderRepository = workOrderRepository;
            _mapper = mapper;
            _mediator = mediator;
            _departmentGrpcClient = departmentGrpcClient;
        }
  
        public async Task<ApiResponseDTO<List<Dictionary<string, List<GetWorkOrderDto>>>>> Handle(GetWorkOrderQuery request, CancellationToken cancellationToken)
        {
           var workOrder = await _workOrderRepository.GetAllWOAsync(request.fromDate,request.toDate, request.requestTypeId, request.departmentId);            
           var mappedWorkOrders = _mapper.Map<List<GetWorkOrderDto>>(workOrder);

             // 🔥 Fetch departments using gRPC
            var departments = await _departmentGrpcClient.GetAllDepartmentAsync(); // ✅ Clean call

            // var departments = departmentResponse.Departments.ToList();
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

            var PreventiveSchedulerDictionary = new Dictionary<int, GetWorkOrderDto>();
            // 🔥 Map department names to preventiveScheduler
            foreach (var data in mappedWorkOrders)
            {
                if (departmentLookup.TryGetValue(data.DepartmentId, out var departmentName) && departmentName != null)
                {
                    data.Department = departmentName;
                }
                PreventiveSchedulerDictionary[data.DepartmentId] = data;
            }

            var groupedWorkOrders = mappedWorkOrders
            .GroupBy(w => w.MaintenanceType ?? "Unknown")
            .Select(g => new Dictionary<string, List<GetWorkOrderDto>>
            {
                [g.Key] = g.ToList()
            })
            .ToList();

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "",        
                actionName: "",
                details: $"WorkOrderDetails details was fetched.",
                module:"WorkOrderDetails"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
           return new ApiResponseDTO<List<Dictionary<string, List<GetWorkOrderDto>>>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = groupedWorkOrders,          
            };     
        }
    }
}