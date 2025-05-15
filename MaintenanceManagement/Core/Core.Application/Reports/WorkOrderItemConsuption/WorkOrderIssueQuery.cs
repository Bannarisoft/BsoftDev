using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Reports.WorkOrderItemConsuption
{
    public class WorkOrderIssueQuery : IRequest<ApiResponseDTO<List<WorkOrderIssueDto>>>
    {
        public DateTimeOffset? IssueFrom { get; set; }
        public DateTimeOffset? IssueTo { get; set; }
        public int MaintenanceTypeId { get; set; }
    }
}