

using System;

namespace Contracts.Commands.Maintenance
{
    public class CreateNextSchedulerCommand
    {
        public Guid CorrelationId { get; set; }
        public int WorkOrderId { get; set; }
        public int PreventiveSchedulerDetailId { get; set; }
    }
}