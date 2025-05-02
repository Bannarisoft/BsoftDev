using System;
using MassTransit;
namespace Contracts.Events.Maintenance
{
    public class NextSchedulerCreatedEvent  : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
       /*  public int WorkOrderId { get; set; }
        public string Reason { get; set; } */
    }
}