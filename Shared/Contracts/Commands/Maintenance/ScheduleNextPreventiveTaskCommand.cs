using System;
using MassTransit;

namespace Contracts.Commands.Maintenance
{
    public class ScheduleNextPreventiveTaskCommand : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public int SchedulerId { get; set; }
    }
   
}