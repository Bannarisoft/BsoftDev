

using Core.Application.Dashboard.Common;
using MediatR;

namespace Core.Application.Dashboard.DashboardQuery
{
    public class DashboardQuery : IRequest<List<ChartDto>>
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? Type { get; set; } // "department" or "machineGroup"
    }
}