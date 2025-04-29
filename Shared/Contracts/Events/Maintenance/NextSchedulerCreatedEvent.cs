using System;
namespace Contracts.Events.Maintenance
{
    public class NextSchedulerCreatedEvent 
    {
        public Guid CorrelationId { get; set; }
        public int WorkOrderId { get; set; }
        public string Reason { get; set; }
    }
}