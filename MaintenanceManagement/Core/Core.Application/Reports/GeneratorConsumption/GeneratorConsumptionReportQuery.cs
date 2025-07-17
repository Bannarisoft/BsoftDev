using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Reports.GeneratorConsumption
{
    public class GeneratorConsumptionReportQuery : IRequest<ApiResponseDTO<List<GeneratorReportDto>>>
    {
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
    }
}