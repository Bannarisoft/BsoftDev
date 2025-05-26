using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IWorkCenter;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WorkCenter.Queries.GetWorkCenter
{
    public class GetWorkCenterQueryHandler : IRequestHandler<GetWorkCenterQuery, ApiResponseDTO<List<WorkCenterDto>>>
    {
        private readonly IWorkCenterQueryRepository _iWorkCenterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IDepartmentGrpcClient _departmentGrpcClient;
        private readonly IUnitGrpcClient _unitGrpcClient;

        public GetWorkCenterQueryHandler(IWorkCenterQueryRepository iWorkCenterQueryRepository, IMapper mapper, IMediator mediator, IDepartmentGrpcClient departmentService, IUnitGrpcClient unitGrpcClient)
        {
            _iWorkCenterQueryRepository = iWorkCenterQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
            _departmentGrpcClient = departmentService;
            _unitGrpcClient = unitGrpcClient;
        }

        public async Task<ApiResponseDTO<List<WorkCenterDto>>> Handle(GetWorkCenterQuery request, CancellationToken cancellationToken)
        {
            var (WorkCenter, totalCount) = await _iWorkCenterQueryRepository.GetAllWorkCenterGroupAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var workCentersgrouplist = _mapper.Map<List<WorkCenterDto>>(WorkCenter);
            // 🔥 Fetch departments using gRPC
            var departments = await _departmentGrpcClient.GetAllDepartmentAsync();
            var units = await _unitGrpcClient.GetAllUnitAsync();

            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);
            var unitLookup = units.ToDictionary(u => u.UnitId, u => u.UnitName);

             // 🔥 Map department & unit names with DataControl to costCenters
            foreach (var dto in workCentersgrouplist)
            {
                if (departmentLookup.TryGetValue(dto.DepartmentId, out var deptName))
                    dto.DepartmentName = deptName;

                if (unitLookup.TryGetValue(dto.UnitId, out var unitName))
                    dto.UnitName = unitName;
            }

            // // 🔥 Map DepartmentName with DataControl and UnitName in one loop
            // var filteredworkCentersgroupDtos = workCentersgrouplist
            //              .Where(p => departmentLookup.ContainsKey(p.DepartmentId))
            //              .Select(p => new WorkCenterDto
            //              {
            //                  DepartmentId = p.DepartmentId,
            //                  DepartmentName = departmentLookup[p.DepartmentId],
            //              })
            //              .ToList();
            
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetWorkCenter",
                actionCode: "Get",
                actionName: WorkCenter.Count().ToString(),
                details: $"WorkCenter details was fetched.",
                module: "WorkCenter"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<WorkCenterDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = workCentersgrouplist,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

    }
}