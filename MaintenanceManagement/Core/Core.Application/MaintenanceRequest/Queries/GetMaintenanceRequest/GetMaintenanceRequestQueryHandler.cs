using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.External.IDepartment;
using Core.Application.Common.Interfaces.IMaintenanceRequest;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequest
{
    public class GetMaintenanceRequestQueryHandler: IRequestHandler<GetMaintenanceRequestQuery, ApiResponseDTO<List<GetMaintenanceRequestDto>>>
    {
        private readonly IMaintenanceRequestQueryRepository _maintenanceRequestQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IDepartmentService _departmentService;

        public GetMaintenanceRequestQueryHandler(
            IMaintenanceRequestQueryRepository maintenanceRequestQueryRepository,
            IMapper mapper,
            IMediator mediator,
            IDepartmentService departmentService)
        {
            _maintenanceRequestQueryRepository = maintenanceRequestQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
            _departmentService = departmentService;
        }

        public async Task<ApiResponseDTO<List<GetMaintenanceRequestDto>>> Handle(GetMaintenanceRequestQuery request, CancellationToken cancellationToken)
        {
            var (maintenanceRequests, totalCount) = await _maintenanceRequestQueryRepository.GetAllMaintenanceRequestAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var maintenanceRequestList = _mapper.Map<List<GetMaintenanceRequestDto>>(maintenanceRequests);

            var departments = await _departmentService.GetAllDepartmentAsync();
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

            var maintenanceRequestDictionary = new Dictionary<int, GetMaintenanceRequestDto>();

                 foreach (var data in maintenanceRequestList)
            {
              
                    if (departmentLookup.TryGetValue(data.DepartmentId, out var departmentName )&& departmentName != null)
                    {
                        data.DepartmentName = departmentName;
                    }

                    maintenanceRequestDictionary[data.DepartmentId] = data;
                
            }

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
                Data = maintenanceRequestList,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}