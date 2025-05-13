using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.External.IDepartment;
using Core.Application.Common.Interfaces.External.IUnit;
using Core.Application.Common.Interfaces.IWorkCenter;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WorkCenter.Queries.GetWorkCenter
{
    public class GetWorkCenterQueryHandler : IRequestHandler<GetWorkCenterQuery,ApiResponseDTO<List<WorkCenterDto>>>
    {
        private readonly IWorkCenterQueryRepository _iWorkCenterQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IDepartmentService _departmentService;
        private readonly IUnitService _unitService;

        public GetWorkCenterQueryHandler(IWorkCenterQueryRepository iWorkCenterQueryRepository, IMapper mapper, IMediator mediator, IDepartmentService departmentService, IUnitService unitService)
        {
            _iWorkCenterQueryRepository = iWorkCenterQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
            _departmentService = departmentService;
            _unitService = unitService;
        }

        public async Task<ApiResponseDTO<List<WorkCenterDto>>> Handle(GetWorkCenterQuery request, CancellationToken cancellationToken)
        {
             var (WorkCenter, totalCount) = await _iWorkCenterQueryRepository.GetAllWorkCenterGroupAsync(request.PageNumber, request.PageSize, request.SearchTerm);
             var workCentersgrouplist = _mapper.Map<List<WorkCenterDto>>(WorkCenter);
               // 🔥 Fetch lookups
            var departments = await _departmentService.GetAllDepartmentAsync();
            var units = await _unitService.GetUnitAutoCompleteAsync();

            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);
            var unitLookup = units.ToDictionary(u => u.UnitId, u => u.UnitName);

            // 🔁 Set DepartmentName and UnitName in one loop
            foreach (var dto in workCentersgrouplist)
            {
                if (departmentLookup.TryGetValue(dto.DepartmentId, out var deptName))
                    dto.DepartmentName = deptName;

                if (unitLookup.TryGetValue(dto.UnitId, out var unitName))
                    dto.UnitName = unitName;
            }

             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetWorkCenter",
                    actionCode: "Get",        
                    actionName: WorkCenter.Count().ToString(),
                    details: $"WorkCenter details was fetched.",
                    module:"WorkCenter"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<WorkCenterDto>> 
            { 
                IsSuccess = true, 
                Message = "Success", 
                Data = workCentersgrouplist ,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
                };
        }

    }
}