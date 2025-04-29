
using System;

namespace Contracts.Events.Maintenance
{
    public class WorkOrderClosedEvent
    {
        public Guid CorrelationId { get; set; }
        public int PreventiveSchedulerDetailId { get; set; }
        public int WorkOrderId { get; set; }
    }
}