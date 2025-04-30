using MassTransit;

namespace SagaOrchestrator.Application.Orchestration.Models
{
    public class WorkOrderSchedulerState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public int WorkOrderId { get; set; }
        public int PreventiveSchedulerDetailId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? FailureReason { get; set; }
    }
}