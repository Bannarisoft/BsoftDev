using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.External.IDepartment;
using Core.Application.Common.Interfaces.External.IUnit;
using Core.Application.Common.Interfaces.IWorkCenter;
using Core.Application.WorkCenter.Queries.GetWorkCenter;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WorkCenter.Queries.GetWorkCenterById
{
    public class GetWorkCenterByIdQueryHandler : IRequestHandler<GetWorkCenterByIdQuery,ApiResponseDTO<WorkCenterDto>>
    {
      
        private readonly IWorkCenterQueryRepository _iWorkCenterQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;  
        private readonly IDepartmentService _departmentService;
        private readonly IUnitService _unitService;

        public GetWorkCenterByIdQueryHandler(IWorkCenterQueryRepository iWorkCenterQueryRepository, IMapper mapper, IMediator mediator, IDepartmentService departmentService, IUnitService unitService)
        {
            _iWorkCenterQueryRepository = iWorkCenterQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
            _departmentService = departmentService;
            _unitService = unitService;
        } 

        public async Task<ApiResponseDTO<WorkCenterDto>> Handle(GetWorkCenterByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _iWorkCenterQueryRepository.GetByIdAsync(request.Id);
            // Check if the entity exists
            if (result is null)
            {
                return new ApiResponseDTO<WorkCenterDto> { IsSuccess = false, Message =$"WorkCenter ID {request.Id} not found." };
            }
            // Map a single entity
             var workCenter = _mapper.Map<WorkCenterDto>(result);
             // 🔥 Fetch lookups
             var departments = await _departmentService.GetAllDepartmentAsync();
             var units = await _unitService.GetUnitAutoCompleteAsync();
             var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);
             var unitLookup = units.ToDictionary(u => u.UnitId, u => u.UnitName);

           if ((departmentLookup.TryGetValue(workCenter.DepartmentId, out var departmentName) && departmentName != null) |
                (unitLookup.TryGetValue(workCenter.UnitId, out var unitName) && unitName != null))
            {
                workCenter.DepartmentName = departmentName;
                workCenter.UnitName = unitName;
            }

          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "GetWorkCenterByIdQuery",        
                    actionName: workCenter.Id.ToString(),
                    details: $"WorkCenter details {workCenter.Id} was fetched.",
                    module:"WorkCenter"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return new ApiResponseDTO<WorkCenterDto> { IsSuccess = true, Message = "Success", Data = workCenter };
        }
    }
}