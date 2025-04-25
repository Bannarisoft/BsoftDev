using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.External.IDepartment;
using Core.Application.Common.Interfaces.IMaintenanceRequest;
using Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequest;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequestById
{
    public class GetMaintenanceRequestByIdQueryHandler : IRequestHandler<GetMaintenanceRequestByIdQuery, ApiResponseDTO<GetMaintenanceRequestDto>>
    {
         private readonly IMaintenanceRequestQueryRepository _maintenanceRequestQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

          private readonly IDepartmentService _departmentService;
           public GetMaintenanceRequestByIdQueryHandler(IMaintenanceRequestQueryRepository maintenanceRequestQueryRepository, IMapper mapper, IMediator mediator , IDepartmentService departmentService)
        {
            _maintenanceRequestQueryRepository = maintenanceRequestQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
            _departmentService = departmentService;
        }

         public async Task<ApiResponseDTO<GetMaintenanceRequestDto>> Handle(GetMaintenanceRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _maintenanceRequestQueryRepository.GetByIdAsync(request.Id);


            if (result is null)
            {
                return new ApiResponseDTO<GetMaintenanceRequestDto>
                {
                    IsSuccess = false,
                    Message = $"MaintenanceRequest with Id {request.Id} not found.",
                    Data = null
                };
            }

                    var maintenanceRequest = _mapper.Map<GetMaintenanceRequestDto>(result);

            var departments = await _departmentService.GetAllDepartmentAsync();
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

            if (departmentLookup.TryGetValue(maintenanceRequest.DepartmentId, out string departmentName) && departmentName != null)
            {
                maintenanceRequest.DepartmentName = departmentName;
            }


            // Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: "",
                actionName: "",
                details: $"MaintenanceRequest details for Id {maintenanceRequest.Id} was fetched.",
                module: "MaintenanceRequest"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<GetMaintenanceRequestDto>
            {
                IsSuccess = true,
                Message = "Success",
                Data = maintenanceRequest
            };
        }
    }
}