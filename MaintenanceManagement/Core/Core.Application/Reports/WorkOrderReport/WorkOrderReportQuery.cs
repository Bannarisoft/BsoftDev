using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Reports.WorkOrderReport
{
    public class WorkOrderReportQuery : IRequest<ApiResponseDTO<List<WorkOrderReportDto>>> 
    {
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public int RequestTypeId { get; set; }
    }
}
 