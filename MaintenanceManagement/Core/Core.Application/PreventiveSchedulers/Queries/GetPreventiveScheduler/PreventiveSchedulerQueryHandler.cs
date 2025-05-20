using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Domain.Events;
using MediatR;
using Contracts.Interfaces.External.IUser;

namespace Core.Application.PreventiveSchedulers.Queries.GetPreventiveScheduler
{
    public class PreventiveSchedulerQueryHandler : IRequestHandler<GetPreventiveSchedulerQuery, ApiResponseDTO<List<GetPreventiveSchedulerDto>>>
    {
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IDepartmentGrpcClient _departmentGrpcClient;

        public PreventiveSchedulerQueryHandler(IPreventiveSchedulerQuery preventiveSchedulerQuery, IMapper mapper, IMediator mediator, IDepartmentGrpcClient departmentGrpcClient)
        {
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
            _mapper = mapper;
            _mediator = mediator;
            _departmentGrpcClient = departmentGrpcClient;

        }
        public async Task<ApiResponseDTO<List<GetPreventiveSchedulerDto>>> Handle(GetPreventiveSchedulerQuery request, CancellationToken cancellationToken)
        {
            var (preventiveScheduler, totalCount) = await _preventiveSchedulerQuery.GetAllPreventiveSchedulerAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var preventiveSchedulerList = _mapper.Map<List<GetPreventiveSchedulerDto>>(preventiveScheduler);

            // 🔥 Fetch departments using gRPC
            var departments = await _departmentGrpcClient.GetAllDepartmentAsync(); // ✅ Clean call

            // var departments = departmentResponse.Departments.ToList();
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

            var PreventiveSchedulerDictionary = new Dictionary<int, GetPreventiveSchedulerDto>();
            // 🔥 Map department names to preventiveScheduler
            foreach (var data in preventiveSchedulerList)
            {

                if (departmentLookup.TryGetValue(data.DepartmentId, out var departmentName) && departmentName != null)
                {
                    data.DepartmentName = departmentName;
                }

                PreventiveSchedulerDictionary[data.DepartmentId] = data;

            }

            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetPreventiveScheduler",
                actionCode: "",
                actionName: "",
                details: $"PreventiveScheduler details was fetched.",
                module: "PreventiveScheduler"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<GetPreventiveSchedulerDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = preventiveSchedulerList,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}