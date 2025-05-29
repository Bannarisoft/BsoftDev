using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IReports;
using MediatR;

namespace Core.Application.Reports.ScheduleReport
{
    public class ScheduleReportQueryHandler : IRequestHandler<ScheduleReportQuery, ApiResponseDTO<List<ScheduleReportDto>>>
    {
        private readonly IReportRepository _reportQueryRepository;
        private readonly IMapper _mapper;
        private readonly IDepartmentGrpcClient _departmentGrpcClient;
        public ScheduleReportQueryHandler(IReportRepository reportQueryRepository, IMapper mapper, IDepartmentGrpcClient departmentGrpcClient)
        {
            _reportQueryRepository = reportQueryRepository;
            _mapper = mapper;
            _departmentGrpcClient = departmentGrpcClient;
        }
        public async Task<ApiResponseDTO<List<ScheduleReportDto>>> Handle(ScheduleReportQuery request, CancellationToken cancellationToken)
        {
            var reportEntities = await _reportQueryRepository.ScheduleReportAsync(request.FromDueDate, request.ToDueDate) ?? new List<ScheduleReportDto>();

            var preventiveSchedulerList = _mapper.Map<List<ScheduleReportDto>>(reportEntities) ?? new List<ScheduleReportDto>();

            var departments = await _departmentGrpcClient.GetAllDepartmentAsync();
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);
            // var PreventiveSchedulerDictionary = new Dictionary<int, ScheduleReportDto>();


            // foreach (var data in preventiveSchedulerList)
            // {

            //     if (departmentLookup.TryGetValue(data.DepartmentId, out var departmentName) && departmentName != null)
            //     {
            //         data.Department = departmentName;
            //     }

            //     PreventiveSchedulerDictionary[data.DepartmentId] = data;

            // }

              foreach (var dto in preventiveSchedulerList)
        {
            if (departmentLookup.TryGetValue(dto.DepartmentId, out var departmentName))
            {
                dto.DepartmentName = departmentName;
            }
        }

                var filteredPreventiveSchedulers = preventiveSchedulerList
            .Where(p => departmentLookup.ContainsKey(p.DepartmentId))
            .ToList();



            return new ApiResponseDTO<List<ScheduleReportDto>>
            {
                IsSuccess = filteredPreventiveSchedulers.Any(),
                Message = filteredPreventiveSchedulers.Any()
                 ? "Scheduler Report retrieved successfully."
                 : "No Scheduler Report found.",
                Data = filteredPreventiveSchedulers
            };
        }
    }
}