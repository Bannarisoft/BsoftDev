

using Core.Application.Dashboard.Common;
using MediatR;

namespace Core.Application.Dashboard.DashboardQuery
{
    public class DashboardQuery : IRequest<ChartDto>
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? Type { get; set; }
        public string? DepartmentId { get; set; }
        public string? MachineGroupId { get; set; }
        public string? ItemCode { get; set; }
    }
}