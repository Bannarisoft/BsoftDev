using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.External.IDepartment;
using Core.Application.Common.Interfaces.IMaintenanceRequest;
using MediatR;

namespace Core.Application.MaintenanceRequest.Queries.GetExternalRequestById
{
    public class GetExternalRequestByIdQueryHandler : IRequestHandler<GetExternalRequestsByIdsQuery, ApiResponseDTO<List<GetExternalRequestByIdDto>>>
    {

        private readonly IMaintenanceRequestQueryRepository _maintenanceRequestQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        private readonly IDepartmentService _departmentService;
        public GetExternalRequestByIdQueryHandler(IMaintenanceRequestQueryRepository maintenanceRequestQueryRepository, IMapper mapper, IMediator mediator, IDepartmentService departmentService)
        {
            _maintenanceRequestQueryRepository = maintenanceRequestQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
            _departmentService = departmentService;
        }

      

                    public async Task<ApiResponseDTO<List<GetExternalRequestByIdDto>>> Handle(GetExternalRequestsByIdsQuery request, CancellationToken cancellationToken)
            {


                if (request.Ids == null || !request.Ids.Any())
                {
                    return new ApiResponseDTO<List<GetExternalRequestByIdDto>>
                    {
                        IsSuccess = false,
                        Message = "No IDs provided.",
                        Data = new List<GetExternalRequestByIdDto>()
                    };
                }

                var externalRequests = await _maintenanceRequestQueryRepository.GetExternalRequestByIdAsync(request.Ids);

                     // Fetch departments and build lookup
                    var departments = await _departmentService.GetAllDepartmentAsync();
                    var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

                    // Assign department names to each external request
                    foreach (var requestDto in externalRequests)
                    {
                        if (requestDto.DepartmentId != 0 && departmentLookup.TryGetValue(requestDto.DepartmentId, out var deptName))
                        {
                            requestDto.DepartmentName = deptName;
                        }
                    }


                if (!externalRequests.Any())
                {
                    return new ApiResponseDTO<List<GetExternalRequestByIdDto>>
                    {
                        IsSuccess = false,
                        Message = "No external requests found for the provided IDs.",
                        Data = new List<GetExternalRequestByIdDto>()
                    };
                }

                return new ApiResponseDTO<List<GetExternalRequestByIdDto>>
                {
                    IsSuccess = true,
                    Message = "External requests fetched successfully.",
                    Data = externalRequests
                };
            }

        
    }
}