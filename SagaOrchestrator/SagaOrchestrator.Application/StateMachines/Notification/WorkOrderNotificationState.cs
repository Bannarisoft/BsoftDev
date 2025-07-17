using MassTransit;
using SagaOrchestrator.Application.Orchestration.Models.Notifications;
using Contracts.Events.Notifications.WorkOrder;
using Contracts.Events.Notifications.WorkOrder.Email;
using Contracts.Events.Notifications.WorkOrder.Sms;
using Contracts.Events.Notifications.WorkOrder.InApp;
using Contracts.Events.Notifications.Internal.Email;


namespace SagaOrchestrator.Application.StateMachines.Notification
{
    public class WorkOrderNotificationState : MassTransitStateMachine<NotificationWorkOrder>
    {
        public State Notifying { get; private set; }
        public State Completed { get; private set; }
        public State Failed { get; private set; }
        public State Rollback { get; private set; }

        public Event<WorkOrderCreatedEvent> WorkOrderCreated { get; private set; }

        public Event<SendEmailNotificationCompleted> EmailCompleted { get; private set; }
        public Event<SendSmsNotificationCompleted> SmsCompleted { get; private set; }
        public Event<SendInAppNotificationCompleted> InAppCompleted { get; private set; }

        public Event<SendEmailNotificationFailed> EmailFailed { get; private set; }
        public Event<SendSmsNotificationFailed> SmsFailed { get; private set; }
        public Event<SendInAppNotificationFailed> InAppFailed { get; private set; }

        public WorkOrderNotificationState()
        {
            InstanceState(x => x.CurrentState);

            Event(() => WorkOrderCreated, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => EmailCompleted, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => SmsCompleted, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => InAppCompleted, x => x.CorrelateById(m => m.Message.CorrelationId));

            Event(() => EmailFailed, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => SmsFailed, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => InAppFailed, x => x.CorrelateById(m => m.Message.CorrelationId));

            Initially(
                When(WorkOrderCreated)
                    .Then(context => context.Instance.CreatedAt = DateTime.UtcNow)
                    .ThenAsync(async context =>
                    {
                        await context.Publish(new SendEmailNotificationInternalCommand
                        {
                            CorrelationId = context.Data.CorrelationId,
                            UnitId = context.Data.UnitId,
                            ModuleName = "WorkOrder",
                            EventTypeId = context.Data.EventTypeId
                        });

                        await context.Publish(new SendSmsNotificationInternalCommand
                        {
                            CorrelationId = context.Data.CorrelationId,
                            UnitId = context.Data.UnitId,
                            ModuleName = "WorkOrder",
                            EventTypeId =context.Data.EventTypeId
                        });

                        await context.Publish(new SendInAppNotificationInternalCommand
                        {
                            CorrelationId = context.Data.CorrelationId,
                            UnitId = context.Data.UnitId,
                            ModuleName = "WorkOrder",
                            EventTypeId = context.Data.EventTypeId
                        });
                    })
                    .TransitionTo(Notifying)
            );

            During(Notifying,
                When(EmailCompleted)
                    .Then(ctx => ctx.Instance.EmailSent = true)
                    .IfElse(
                        context => AllChannelsHandled(context.Instance),
                        binder => binder.TransitionTo(Completed),
                        binder => binder
                    ),

                When(SmsCompleted)
                    .Then(ctx => ctx.Instance.SmsSent = true)
                    .IfElse(
                        context => AllChannelsHandled(context.Instance),
                        binder => binder.TransitionTo(Completed),
                        binder => binder
                    ),

                When(InAppCompleted)
                    .Then(ctx => ctx.Instance.InAppSent = true)
                    .IfElse(
                        context => AllChannelsHandled(context.Instance),
                        binder => binder.TransitionTo(Completed),
                        binder => binder
                    ),

                When(EmailFailed)
                    .Then(ctx =>
                    {
                        ctx.Instance.EmailFailed = true;
                        ctx.Instance.FailureReason = ctx.Data.Reason;
                    })
                    .ThenAsync(TriggerRollback)
                    .TransitionTo(Rollback),

                When(SmsFailed)
                    .Then(ctx =>
                    {
                        ctx.Instance.SmsFailed = true;
                        ctx.Instance.FailureReason = ctx.Data.Reason;
                    })
                    .ThenAsync(TriggerRollback)
                    .TransitionTo(Rollback),

                When(InAppFailed)
                    .Then(ctx =>
                    {
                        ctx.Instance.InAppFailed = true;
                        ctx.Instance.FailureReason = ctx.Data.Reason;
                    })
                    .ThenAsync(TriggerRollback)
                    .TransitionTo(Rollback)
            );
        }

        private bool AllChannelsHandled(NotificationWorkOrder instance)
        {
            return (instance.EmailSent || instance.EmailFailed)
                && (instance.SmsSent || instance.SmsFailed)
                && (instance.InAppSent || instance.InAppFailed);
        }

        private Task TriggerRollback(BehaviorContext<NotificationWorkOrder> context)
        {
            var instance = context.Instance;

            return context.Publish(new NotificationSagaRollbackTriggered
            {
                CorrelationId = instance.CorrelationId,
                Reason = instance.FailureReason
            });
        }
    }
}
