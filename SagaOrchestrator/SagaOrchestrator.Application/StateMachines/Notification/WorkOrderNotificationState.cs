using Contracts.Events.Notifications.WorkOrder;
using Contracts.Events.Notifications.WorkOrder.Email;
using Contracts.Events.Notifications.WorkOrder.InApp;
using Contracts.Events.Notifications.WorkOrder.Sms;
using MassTransit;
using SagaOrchestrator.Application.Orchestration.Models.Notifications;

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
                When(SmsCompleted).Then(ctx => Console.WriteLine("⚠️ SmsCompleted received too early, ignoring...")).Finalize(),
                When(EmailCompleted).Then(ctx => Console.WriteLine("⚠️ EmailCompleted received too early, ignoring...")).Finalize(),
                When(InAppCompleted).Then(ctx => Console.WriteLine("⚠️ InAppCompleted received too early, ignoring...")).Finalize(),

                When(WorkOrderCreated)
                    .Then(ctx =>
                    {
                        ctx.Instance.CreatedAt = DateTime.UtcNow;
                        ctx.Instance.CorrelationId = ctx.Data.CorrelationId;
                        ctx.Instance.UnitId = ctx.Data.UnitId;
                        ctx.Instance.EventTypeId = ctx.Data.EventTypeId;
                        ctx.Instance.ModuleName = "WorkOrder";
                    })
                    .Activity(x => x.OfType<ResolveAndPublishNotificationActivity>()) // ✅ Custom activity
                    .TransitionTo(Notifying)
            );

            During(Notifying,
                When(EmailCompleted)
                    .Then(ctx => ctx.Instance.EmailSent = true)
                    .IfElse(ctx => AllChannelsHandled(ctx.Instance), binder => binder.TransitionTo(Completed), binder => binder),

                When(SmsCompleted)
                    .Then(ctx => ctx.Instance.SmsSent = true)
                    .IfElse(ctx => AllChannelsHandled(ctx.Instance), binder => binder.TransitionTo(Completed), binder => binder),

                When(InAppCompleted)
                    .Then(ctx => ctx.Instance.InAppSent = true)
                    .IfElse(ctx => AllChannelsHandled(ctx.Instance), binder => binder.TransitionTo(Completed), binder => binder),

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

            During(Rollback,
                Ignore(EmailCompleted),
                Ignore(SmsCompleted),
                Ignore(InAppCompleted),
                Ignore(EmailFailed),
                Ignore(SmsFailed),
                Ignore(InAppFailed)
            );
        }

        private bool AllChannelsHandled(NotificationWorkOrder instance)
        {
            return (instance.EmailSent || instance.EmailFailed)
                && (instance.SmsSent || instance.SmsFailed)
                && (instance.InAppSent || instance.InAppFailed);
        }

        private Task TriggerRollback(BehaviorContext<NotificationWorkOrder> ctx)
        {
            var publishEndpoint = ctx.GetPayload<IPublishEndpoint>();
            return publishEndpoint.Publish(new NotificationSagaRollbackTriggered
            {
                CorrelationId = ctx.Instance.CorrelationId,
                Reason = ctx.Instance.FailureReason
            });
        }
    }
}
