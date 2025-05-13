using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.External.IDepartment;
using Core.Application.Common.Interfaces.External.IUnit;
using Core.Application.Common.Interfaces.ICostCenter;
using Core.Application.CostCenter.Queries.GetCostCenter;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.CostCenter.Queries.GetCostCenterById
{
    public class GetCostCenterByIdQueryHandler : IRequestHandler<GetCostCenterByIdQuery,ApiResponseDTO<CostCenterDto>>
    {
        
        private readonly ICostCenterQueryRepository _iCostCenterQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IDepartmentService _departmentService;
        private readonly IUnitService _unitService;


        public GetCostCenterByIdQueryHandler(ICostCenterQueryRepository iCostCenterQueryRepository, IMapper mapper, IMediator mediator, IDepartmentService departmentService, IUnitService unitService)
        {
            _iCostCenterQueryRepository = iCostCenterQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
            _departmentService = departmentService;
            _unitService = unitService;
        }

        public async Task<ApiResponseDTO<CostCenterDto>> Handle(GetCostCenterByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _iCostCenterQueryRepository.GetByIdAsync(request.Id);
            // Check if the entity exists
            if (result is null)
            {
                return new ApiResponseDTO<CostCenterDto> { IsSuccess = false, Message =$"CostCenter ID {request.Id} not found." };
            }
            // Map a single entity
            var costCenter = _mapper.Map<CostCenterDto>(result);
          
          
             var departments = await _departmentService.GetAllDepartmentAsync();
             var units = await _unitService.GetUnitAutoCompleteAsync();
             var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);
             var unitLookup = units.ToDictionary(u => u.UnitId, u => u.UnitName);

           if ((departmentLookup.TryGetValue(costCenter.DepartmentId, out var departmentName) && departmentName != null) |
                (unitLookup.TryGetValue(costCenter.UnitId, out var unitName) && unitName != null))
            {
                costCenter.DepartmentName = departmentName;
                costCenter.UnitName = unitName;
            }

          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "GetCostCenterByIdQuery",        
                    actionName: costCenter.Id.ToString(),
                    details: $"CostCenter details {costCenter.Id} was fetched.",
                    module:"CostCenter"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return new ApiResponseDTO<CostCenterDto> { IsSuccess = true, Message = "Success", Data = costCenter };
        }

    }
}