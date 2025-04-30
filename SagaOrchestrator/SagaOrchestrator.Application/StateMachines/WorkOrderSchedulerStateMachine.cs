using Contracts.Commands.Maintenance;
using Contracts.Events;
using Contracts.Events.Maintenance;
using MassTransit;
using SagaOrchestrator.Application.Orchestration.Models;
using Serilog;

namespace SagaOrchestrator.Application.StateMachines
{
    public class WorkOrderSchedulerStateMachine : MassTransitStateMachine<WorkOrderSchedulerState>
    {
     public State CreatingNextScheduler { get; private set; }
        public State Failed { get; private set; }

        public Event<WorkOrderClosedEvent> WorkOrderClosedEvent { get; private set; }
        public Event<NextSchedulerCreatedEvent> NextSchedulerCreatedEvent { get; private set; }
        public Event<NextSchedulerCreationFailedEvent> NextSchedulerCreationFailedEvent { get; private set; }

        public WorkOrderSchedulerStateMachine()
        {
            InstanceState(x => x.CurrentState);

            // ✅ Configure correlation for WorkOrderClosedEvent and create new saga instance
            Event(() => WorkOrderClosedEvent, x =>
            {
                x.CorrelateById(context => context.Message.CorrelationId);
                x.InsertOnInitial = true;
              /*   x.SetSagaFactory(context => new WorkOrderSchedulerState
                {
                    CorrelationId = context.Message.CorrelationId,
                    WorkOrderId = context.Message.WorkOrderId,
                    PreventiveSchedulerDetailId = context.Message.PreventiveSchedulerDetailId,
                    CreatedAt = DateTime.UtcNow
                }); */
            });

           /*  // ✅ Configure correlation for success and failure responses
            Event(() => NextSchedulerCreatedEvent, x =>
            {
                x.CorrelateById(context => context.Message.CorrelationId);
                x.OnMissingInstance(m => m.Discard());
            });

            Event(() => NextSchedulerCreationFailedEvent, x =>
            {
                x.CorrelateById(context => context.Message.CorrelationId);
                x.OnMissingInstance(m => m.Discard());
            }); */
            Event(() => NextSchedulerCreatedEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
            Event(() => NextSchedulerCreationFailedEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));

            // ✅ Initial state: Start scheduling
            Initially(
                When(WorkOrderClosedEvent)
                    .Then(context =>
                    {
                        context.Instance.WorkOrderId = context.Data.WorkOrderId;
                        context.Instance.PreventiveSchedulerDetailId = context.Data.PreventiveSchedulerDetailId;
                    })
                    .Send(new Uri("queue:schedule-next-task-queue"), context => new ScheduleNextPreventiveTaskCommand
                    {
                        CorrelationId = context.Instance.CorrelationId,
                        SchedulerId = context.Instance.PreventiveSchedulerDetailId
                    })
                    .TransitionTo(CreatingNextScheduler)
            );

            // ✅ Success: transition to Final
            During(CreatingNextScheduler,
                When(NextSchedulerCreatedEvent)
         /*            .Then(async context =>
                    {
                        await context.Publish(new SagaStatusUpdatedEvent
                        {
                            CorrelationId = context.Instance.CorrelationId,
                            WorkOrderId = context.Instance.WorkOrderId,
                            Status = "Completed"
                        });
                    }) */
                    .Finalize(),

                // ✅ Failure: trigger rollback and move to Failed
                When(NextSchedulerCreationFailedEvent)
                       /*  .ThenAsync(async context =>
                    {
                    await context.Publish(new SagaStatusUpdatedEvent
                        {
                            CorrelationId = context.Data.CorrelationId,
                            WorkOrderId = context.Instance.WorkOrderId,
                            Status = "Failed",
                            FailureReason = context.Data.Reason
                        }); */
                        .Send(ctx => new Uri("queue:rollback-workorder-queue"), ctx => new RollbackWorkOrderCommand
                        {
                            CorrelationId = ctx.Data.CorrelationId,
                            WorkOrderId = ctx.Instance.WorkOrderId,
                            Reason = ctx.Data.Reason                        
                        })
                    .TransitionTo(Failed)
            );

            SetCompletedWhenFinalized();
        }
    }
}
