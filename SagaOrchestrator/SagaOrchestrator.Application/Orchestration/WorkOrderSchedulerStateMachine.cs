using Contracts.Commands;
using Contracts.Events.Maintenance;
using MassTransit;
using SagaOrchestrator.Application.Orchestration.Models;
using Serilog;

namespace SagaOrchestrator.Application.Orchestration
{
    public class WorkOrderSchedulerStateMachine : MassTransitStateMachine<WorkOrderSchedulerState>
    {
        public State SchedulerCreationInProgress { get; private set; }
        public State Failed { get; private set; }

        public Event<WorkOrderClosedEvent> WorkOrderClosedEvent { get; private set; }
        public Event<NextSchedulerCreationFailedEvent> NextSchedulerCreationFailedEvent { get; private set; }

        public WorkOrderSchedulerStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => WorkOrderClosedEvent, x =>
            {
                x.CorrelateById(c => c.Message.CorrelationId);
                x.OnMissingInstance(m => m.Discard());
            });

            Event(() => NextSchedulerCreationFailedEvent, x =>
            {
                x.CorrelateById(c => c.Message.CorrelationId);
                x.OnMissingInstance(m => m.Discard());
            });

            Initially(
                When(WorkOrderClosedEvent)
                    .Then(context =>
                    {
                        context.Saga.WorkOrderId = context.Message.WorkOrderId;
                        context.Saga.PreventiveSchedulerDetailId = context.Message.PreventiveSchedulerDetailId;

                        Log.Information("🟢 Saga Started for WorkOrderId: {WorkOrderId}", context.Saga.WorkOrderId);
                    })
                    .Publish(context => new CreateNextSchedulerCommand
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        PreventiveSchedulerDetailId = context.Saga.PreventiveSchedulerDetailId,
                        WorkOrderId = context.Saga.WorkOrderId
                    })
                    .TransitionTo(SchedulerCreationInProgress)
            );

           During(SchedulerCreationInProgress,
    Ignore(WorkOrderClosedEvent), // ✅ Safely ignore duplicate event

    When(NextSchedulerCreationFailedEvent)
        .Then(context =>
        {
            Log.Error("❌ Scheduler creation failed for WorkOrderId: {WorkOrderId}. Reason: {Reason}",
                context.Saga.WorkOrderId, context.Message.Reason);
        })
        .TransitionTo(Failed)
);
        }
    }
}
