using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IReports;
using MediatR;

namespace Core.Application.Reports.ScheduleReport
{
    public class ScheduleReportQueryHandler : IRequestHandler<ScheduleReportQuery, ApiResponseDTO<List<ScheduleReportDto>>>
    { 
         private readonly IReportRepository _reportQueryRepository;
        private readonly IMapper _mapper;
        //  private readonly IDepartmentService _departmentService;
        public ScheduleReportQueryHandler(IReportRepository reportQueryRepository, IMapper mapper)
        {
            _reportQueryRepository = reportQueryRepository;
            _mapper = mapper;
            // _departmentService = departmentService;
        }
        public async Task<ApiResponseDTO<List<ScheduleReportDto>>> Handle(ScheduleReportQuery request, CancellationToken cancellationToken)
        {
            var reportEntities = await _reportQueryRepository.ScheduleReportAsync(request.FromDueDate, request.ToDueDate)?? new List<ScheduleReportDto>();   

            var preventiveSchedulerList = _mapper.Map<List<ScheduleReportDto>>(reportEntities)?? new List<ScheduleReportDto>();

            // var departments = await _departmentService.GetAllDepartmentAsync();
            // var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);
            var PreventiveSchedulerDictionary = new Dictionary<int, ScheduleReportDto>();


            foreach (var data in preventiveSchedulerList)
            {

                // if (departmentLookup.TryGetValue(data.DepartmentId, out var departmentName) && departmentName != null)
                // {
                //     data.Department = departmentName;
                // }

                PreventiveSchedulerDictionary[data.DepartmentId] = data;

            }

            
               
                return new ApiResponseDTO<List<ScheduleReportDto>>
                {
                  IsSuccess =preventiveSchedulerList.Any(),
                     Message =preventiveSchedulerList.Any()
                     ? "Scheduler Report retrieved successfully."
                     : "No Scheduler Report found.",
                     Data = preventiveSchedulerList
                };
        }
    }
}