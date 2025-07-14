using System;
using MassTransit;

namespace Contracts.Events.Maintenance.Notifications.WorkOrder
{
    public class WorkOrderCreatedEvent : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public int DepartmentId { get; set; }
        public int WorkOrderId { get; set; }
        public string WorkOrderTitle { get; set; }
        public string CreatedByName { get; set; }
    }
}