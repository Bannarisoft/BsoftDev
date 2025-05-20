using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMaintenanceRequest;
using Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequest;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MaintenanceRequest.Queries.GetMaintenanceExternalRequest
{
    public class GetMaintenanceExternalRequestQueryHandler : IRequestHandler<GetMaintenanceExternalRequestQuery, ApiResponseDTO<List<GetMaintenanceExternalRequestDto>>>
    {

        private readonly IMaintenanceRequestQueryRepository _maintenanceRequestQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;        
        private readonly IDepartmentGrpcClient _departmentGrpcClient;


        public GetMaintenanceExternalRequestQueryHandler(
            IMaintenanceRequestQueryRepository maintenanceRequestQueryRepository,
            IMapper mapper,
            IMediator mediator ,
            IDepartmentGrpcClient departmentGrpcClient
            )
        {
            _maintenanceRequestQueryRepository = maintenanceRequestQueryRepository;
            _mapper = mapper;
            _mediator = mediator;            
            _departmentGrpcClient = departmentGrpcClient;

        }

        public async Task<ApiResponseDTO<List<GetMaintenanceExternalRequestDto>>> Handle(GetMaintenanceExternalRequestQuery request, CancellationToken cancellationToken)
        {
            var (maintenanceExternalRequests, totalCount) = await _maintenanceRequestQueryRepository.GetAllMaintenanceExternalRequestAsync(request.PageNumber, request.PageSize, request.SearchTerm ,   request.FromDate,request.ToDate);
            var maintenanceRequestList = _mapper.Map<List<GetMaintenanceExternalRequestDto>>(maintenanceExternalRequests);

           
            var departments = await _departmentGrpcClient.GetAllDepartmentAsync(); // ✅ Clean call
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

            var maintenanceRequestDictionary = new Dictionary<int, GetMaintenanceExternalRequestDto>();
            
           
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