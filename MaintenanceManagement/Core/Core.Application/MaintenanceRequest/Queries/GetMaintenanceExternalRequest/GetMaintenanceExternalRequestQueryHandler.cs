using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMaintenanceRequest;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MaintenanceRequest.Queries.GetMaintenanceExternalRequest
{
    public class GetMaintenanceExternalRequestQueryHandler : IRequestHandler<GetMaintenanceExternalRequestQuery, ApiResponseDTO<List<GetMaintenanceExternalRequestDto>>>
    {

        private readonly IMaintenanceRequestQueryRepository _maintenanceRequestQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        // private readonly IDepartmentGrpcClient _departmentGrpcClient; // ✅ Interface, not DepartmentServiceClient


        public GetMaintenanceExternalRequestQueryHandler(
            IMaintenanceRequestQueryRepository maintenanceRequestQueryRepository,
            IMapper mapper,
            IMediator mediator
            // IDepartmentGrpcClient departmentGrpcClient
            )
        {
            _maintenanceRequestQueryRepository = maintenanceRequestQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
            // _departmentGrpcClient = departmentGrpcClient;

        }

        public async Task<ApiResponseDTO<List<GetMaintenanceExternalRequestDto>>> Handle(GetMaintenanceExternalRequestQuery request, CancellationToken cancellationToken)
        {
            var (maintenanceExternalRequests, totalCount) = await _maintenanceRequestQueryRepository.GetAllMaintenanceExternalRequestAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var maintenanceRequestList = _mapper.Map<List<GetMaintenanceExternalRequestDto>>(maintenanceExternalRequests);

            // 🔥 Fetch departments using gRPC
            // var departments = await _departmentGrpcClient.GetAllDepartmentsAsync();
            // var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

            // var maintenanceRequestDictionary = new Dictionary<int, GetMaintenanceExternalRequestDto>();
            
            // // 🔥 Map department names to locations
            // foreach (var data in maintenanceRequestList)
            // {

            //     if (departmentLookup.TryGetValue(data.DepartmentId, out var departmentName) && departmentName != null)
            //     {
            //         data.DepartmentName = departmentName;
            //     }

            //     maintenanceRequestDictionary[data.DepartmentId] = data;

            // }

            // Domain Event Logging
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "",
                actionName: "",
                details: "MaintenanceRequest records were fetched.",
                module: "MaintenanceRequest"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<List<GetMaintenanceExternalRequestDto>>
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