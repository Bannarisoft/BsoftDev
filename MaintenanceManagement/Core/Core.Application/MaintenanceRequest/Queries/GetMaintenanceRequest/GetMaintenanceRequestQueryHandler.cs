using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMaintenanceRequest;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequest
{
    public class GetMaintenanceRequestQueryHandler : IRequestHandler<GetMaintenanceRequestQuery, ApiResponseDTO<List<GetMaintenanceRequestDto>>>
    {
        private readonly IMaintenanceRequestQueryRepository _maintenanceRequestQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        private readonly IDepartmentGrpcClient _departmentGrpcClient;


        public GetMaintenanceRequestQueryHandler(
            IMaintenanceRequestQueryRepository maintenanceRequestQueryRepository,
            IMapper mapper,
            IMediator mediator,
            IDepartmentGrpcClient departmentService)

        {
            _maintenanceRequestQueryRepository = maintenanceRequestQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
            _departmentGrpcClient = departmentService;


        }

        public async Task<ApiResponseDTO<List<GetMaintenanceRequestDto>>> Handle(GetMaintenanceRequestQuery request, CancellationToken cancellationToken)
        {
            var (maintenanceRequests, totalCount) = await _maintenanceRequestQueryRepository.GetAllMaintenanceRequestAsync(request.PageNumber, request.PageSize, request.SearchTerm, request.FromDate, request.ToDate);
            var maintenanceRequestList = _mapper.Map<List<GetMaintenanceRequestDto>>(maintenanceRequests);

            // 🔥 Fetch departments using gRPC
            var departments = await _departmentGrpcClient.GetAllDepartmentAsync(); // ✅ Clean call
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

            var maintenanceRequestDictionary = new Dictionary<int, GetMaintenanceRequestDto>();

            // 🔥 Map department names with DataControl to location

           foreach (var data in maintenanceRequestList)
            {

                if (departmentLookup.TryGetValue(data.ProductionDepartmentId, out var departmentName) && departmentName != null)
                {

                    data.ProductionDepartmentName  = departmentName;
                }
                maintenanceRequestDictionary[data.ProductionDepartmentId] = data;

            }

               var filteredMaintenanceRequest = maintenanceRequestList
            .Where(p => departmentLookup.ContainsKey(p.ProductionDepartmentId))
            .ToList();
            
            // var filteredmaintenanceRequestDtos = maintenanceRequestList
            //  .Where(p => departmentLookup.ContainsKey(p.DepartmentId))
            //  .Select(p => new GetMaintenanceRequestDto
            //  {
            //      DepartmentId = p.DepartmentId,
            //      DepartmentName = departmentLookup[p.DepartmentId],
            //  })
            //  .ToList();

            // Domain Event Logging
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "",
                actionName: "",
                details: "MaintenanceRequest records were fetched.",
                module: "MaintenanceRequest"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<List<GetMaintenanceRequestDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = filteredMaintenanceRequest,
                TotalCount = filteredMaintenanceRequest.Count,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}