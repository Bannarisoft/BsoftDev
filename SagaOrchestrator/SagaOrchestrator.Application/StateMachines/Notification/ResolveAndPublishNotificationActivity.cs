using Contracts.Events.Notifications.WorkOrder;
using Contracts.Events.Notifications.WorkOrder.Email;
using Contracts.Events.Notifications.WorkOrder.Sms;
using Contracts.Events.Notifications.WorkOrder.InApp;
using MassTransit;
using MassTransit.Courier;
using MassTransit.Saga;
using SagaOrchestrator.Application.Orchestration.Models.Notifications;

namespace SagaOrchestrator.Application.StateMachines.Notification
{
    public class ResolveAndPublishNotificationActivity : IStateMachineActivity<NotificationWorkOrder, WorkOrderCreatedEvent>
    {
        private readonly IRequestClient<SendNotificationInternalCommand> _requestClient;
        private readonly IPublishEndpoint _publishEndpoint;

        public ResolveAndPublishNotificationActivity(
            IRequestClient<SendNotificationInternalCommand> requestClient,
            IPublishEndpoint publishEndpoint)
        {
            _requestClient = requestClient;
            _publishEndpoint = publishEndpoint;
        }

        public void Probe(ProbeContext context) => context.CreateScope("resolve-and-publish");

        public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);

        public async Task Execute(BehaviorContext<NotificationWorkOrder, WorkOrderCreatedEvent> context, IBehavior<NotificationWorkOrder, WorkOrderCreatedEvent> next)
        {
            var instance = context.Instance;
            var message = context.Data;

            var response = await _requestClient.GetResponse<ResolveNotificationChannelsResponse>(
                new SendNotificationInternalCommand
                {
                    CorrelationId = message.CorrelationId,
                    UnitId = message.UnitId,
                    EventTypeId = message.EventTypeId,
                    ModuleName = "WorkOrder"
                });

            var channels = response.Message.Channels;

            if (channels.Contains("Email", StringComparer.OrdinalIgnoreCase))
            {
                await _publishEndpoint.Publish(new SendEmailNotificationInternalCommand
                {
                    CorrelationId = message.CorrelationId,
                    UnitId = message.UnitId,
                    EventTypeId = message.EventTypeId,
                    ModuleName = "WorkOrder"
                });
            }

            if (channels.Contains("SMS", StringComparer.OrdinalIgnoreCase))
            {
                await _publishEndpoint.Publish(new SendSmsNotificationInternalCommand
                {
                    CorrelationId = message.CorrelationId,
                    UnitId = message.UnitId,
                    EventTypeId = message.EventTypeId,
                    ModuleName = "WorkOrder"
                });
            }

            if (channels.Contains("InApp", StringComparer.OrdinalIgnoreCase))
            {
                await _publishEndpoint.Publish(new SendInAppNotificationInternalCommand
                {
                    CorrelationId = message.CorrelationId,
                    UnitId = message.UnitId,
                    EventTypeId = message.EventTypeId,
                    ModuleName = "WorkOrder"
                });
            }

            await next.Execute(context);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<NotificationWorkOrder, WorkOrderCreatedEvent, TException> context, IBehavior<NotificationWorkOrder, WorkOrderCreatedEvent> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}