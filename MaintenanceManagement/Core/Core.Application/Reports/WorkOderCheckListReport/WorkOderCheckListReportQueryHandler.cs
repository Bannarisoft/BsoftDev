using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.External.IDepartment;
using Core.Application.Common.Interfaces.External.IUnit;
using Core.Application.Common.Interfaces.IReports;
using MediatR;

namespace Core.Application.Reports.WorkOderCheckListReport
{
    public class WorkOderCheckListReportQueryHandler : IRequestHandler<WorkOderCheckListReportQuery, ApiResponseDTO<List<WorkOderCheckListReportDto>>>
    {

        private readonly IReportRepository _workOrderCheckListQueryRepository;
        private readonly IMapper _mapper;
        private readonly IDepartmentService _departmentService;
        private readonly IUnitService _unitService;

        public WorkOderCheckListReportQueryHandler(IReportRepository workOrderCheckListQueryRepository, IMapper mapper, IUnitService unitService, IDepartmentService departmentService)
        {
            _workOrderCheckListQueryRepository = workOrderCheckListQueryRepository;
            _mapper = mapper;
            _unitService = unitService;
            _departmentService = departmentService;
        }
        
             public async Task<ApiResponseDTO<List<WorkOderCheckListReportDto>>> Handle(WorkOderCheckListReportQuery request, CancellationToken cancellationToken)
        {

            var requestReportEntities = await _workOrderCheckListQueryRepository.GetWorkOrderChecklistReportAsync(
            request.WorkOrderFromDate,
            request.WorkOrderToDate,
            request.MachineGroupId,
            request.MachineId,
            request.ActivityId                             

           );

            var requestReportDtos = _mapper.Map<List<WorkOderCheckListReportDto>>(requestReportEntities);

            // Step 3: Fetch department and unit data
            var departments = await _departmentService.GetAllDepartmentAsync();
            var units = await _unitService.GetUnitAutoCompleteAsync();
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);
            var unitLookup = units.ToDictionary(u => u.UnitId, u => u.UnitName);
           
            // Step 4: Assign DepartmentName and UnitName to each DTO
            foreach (var dto in requestReportDtos)
            {
                // if (departmentLookup.TryGetValue(dto.DepartmentId, out var departmentName))
                // {
                //     dto.Department = departmentName;
                // }
                if (unitLookup.TryGetValue(dto.UnitId, out var unitName))
                {
                    dto.UnitName = unitName;
                }
            }
            return new ApiResponseDTO<List<WorkOderCheckListReportDto>>
            {
                IsSuccess = requestReportDtos != null && requestReportDtos.Any(),
                Message = requestReportDtos != null && requestReportDtos.Any()
                    ? "WorkOderCheckList report retrieved successfully."
                    : "No WorkOderCheckList requests found.",
                Data = requestReportDtos 
            };
        }

        
    }
}