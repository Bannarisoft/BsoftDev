using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Reports.MRS
{
    public class MRSReportQuery : IRequest<ApiResponseDTO<List<MRSReportDto>>>
    {
        public DateTime? IssueFrom { get; set; }
        public DateTime? IssueTo { get; set; }
        public string? OldUnitCode { get; set; }
    }
}