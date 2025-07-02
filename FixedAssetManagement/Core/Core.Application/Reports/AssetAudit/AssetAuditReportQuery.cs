using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Reports.AssetAudit
{
    public class AssetAuditReportQuery : IRequest<ApiResponseDTO<List<AssetAuditReportDto>>>
    {
        public int AuditCycle { get; set; }
    }
}