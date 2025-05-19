using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Reports.ScheduleReport
{
    public class ScheduleReportQuery : IRequest<ApiResponseDTO<List<ScheduleReportDto>>>
    {
        public string? MachineDepartment { get; set; }
        public string? MachineGroup { get; set; }
        public string? MaintenanceCategory { get; set; }
        public string? Activity { get; set; }
        public string? ActivityType { get; set; }
    }
}