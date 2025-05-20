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
        public DateTime? FromDueDate { get; set; }
        public DateTime? ToDueDate { get; set; }
    }
}