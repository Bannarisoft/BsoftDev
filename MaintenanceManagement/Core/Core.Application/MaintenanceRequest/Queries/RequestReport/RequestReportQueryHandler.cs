using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMaintenanceRequest;
using MediatR;

namespace Core.Application.MaintenanceRequest.Queries.RequestReport
{
    public class RequestReportQueryHandler : IRequestHandler<RequestReportQuery, ApiResponseDTO<List<RequestReportDto>>>
    {
        
        private readonly IMaintenanceRequestQueryRepository _maintenanceRequestQueryRepository;
        private readonly IMapper _mapper;

        public RequestReportQueryHandler( IMaintenanceRequestQueryRepository maintenanceRequestQueryRepository, IMapper mapper)
        {
            _maintenanceRequestQueryRepository = maintenanceRequestQueryRepository;
            _mapper = mapper;
        }

          public async Task<ApiResponseDTO<List<RequestReportDto>>> Handle(RequestReportQuery request, CancellationToken cancellationToken)
        {
                    var requestReportEntities = await _maintenanceRequestQueryRepository.MaintenanceReportAsync(
                    request.RequestFromDate,
                    request.RequestToDate,
                    request.RequestType,
                    request.RequestStatus);

                var requestReportDtos = _mapper.Map<List<RequestReportDto>>(requestReportEntities);

                return new ApiResponseDTO<List<RequestReportDto>>
                {
                    IsSuccess = requestReportDtos != null && requestReportDtos.Any(),
                    Message = requestReportDtos != null && requestReportDtos.Any()
                        ? "Maintenance report retrieved successfully."
                        : "No maintenance requests found.",
                    Data = requestReportDtos
                };
        }



        
    }
}