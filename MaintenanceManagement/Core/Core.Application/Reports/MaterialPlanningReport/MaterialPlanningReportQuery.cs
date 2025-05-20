using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Reports.MaterialPlanningReport
{
    public class MaterialPlanningReportQuery : IRequest<ApiResponseDTO<List<MaterialPlanningReportDto>>>
    {
        public DateTime? FromDueDate { get; set; }
        public DateTime? ToDueDate { get; set; }
    }
}