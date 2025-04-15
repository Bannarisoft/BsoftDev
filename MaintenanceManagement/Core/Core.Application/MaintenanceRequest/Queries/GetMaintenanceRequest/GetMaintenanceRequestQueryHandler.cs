using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
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

        public GetMaintenanceRequestQueryHandler(
            IMaintenanceRequestQueryRepository maintenanceRequestQueryRepository,
            IMapper mapper,
            IMediator mediator)
        {
            _maintenanceRequestQueryRepository = maintenanceRequestQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<GetMaintenanceRequestDto>>> Handle(GetMaintenanceRequestQuery request, CancellationToken cancellationToken)
        {
            var (maintenanceRequests, totalCount) = await _maintenanceRequestQueryRepository.GetAllMaintenanceRequestAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var maintenanceRequestList = _mapper.Map<List<GetMaintenanceRequestDto>>(maintenanceRequests);

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