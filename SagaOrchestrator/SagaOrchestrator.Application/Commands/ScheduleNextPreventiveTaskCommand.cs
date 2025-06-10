using MassTransit;

namespace SagaOrchestrator.Application.Commands
{
    public class ScheduleNextPreventiveTaskCommand : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public int SchedulerId { get; set; }
        public Guid UserId { get; set; }
    }
}