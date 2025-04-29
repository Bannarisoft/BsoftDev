

using System;

namespace Contracts.Commands
{
    public class CreateNextSchedulerCommand
    {
        public Guid CorrelationId { get; set; }
        public int WorkOrderId { get; set; }
        public int PreventiveSchedulerDetailId { get; set; }
    }
}