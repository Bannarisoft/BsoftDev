using MassTransit;

namespace SagaOrchestrator.Application.Orchestration.Models
{
    public class WorkOrderSchedulerState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public int WorkOrderId { get; set; }
        public int PreventiveSchedulerDetailId { get; set; }
        public int Version { get; set; }
    }
}