using MassTransit;

namespace SagaOrchestrator.Application.StateMachines
{
    public class WorkOrderSchedulerStateMachinedel : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = string.Empty;
        public int WorkOrderId { get; set; }
        public int PreventiveSchedulerDetailId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}