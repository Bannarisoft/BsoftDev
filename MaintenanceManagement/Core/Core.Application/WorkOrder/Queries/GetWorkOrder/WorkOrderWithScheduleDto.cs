using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.WorkOrder.Queries.GetWorkOrder
{
    public class WorkOrderWithScheduleDto
    {
        public int Id { get; set; }
        public string? WorkOrderDocNo { get; set; }
        public string? Department { get; set; }
        public string? Machine { get; set; }
        public string? RequestDate { get; set; }
        public string? RequestType { get; set; }
        public string? Status { get; set; }
        public string? MaintenanceType { get; set; }
        public int RequestId { get; set; }
        public DateTimeOffset? ScheduleStartTime { get; set; }
        public DateTimeOffset? ScheduleEndTime { get; set; }
    }
}