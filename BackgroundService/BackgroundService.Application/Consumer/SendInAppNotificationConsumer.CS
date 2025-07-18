using BackgroundService.Application.Interfaces.Notification;
using BackgroundService.Application.Notification;
using Contracts.Events.Notifications.WorkOrder.InApp;
using Hangfire;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BackgroundService.Application.Consumers
{
    public class SendInAppNotificationConsumer : IConsumer<SendInAppNotificationInternalCommand>
    {
        private readonly IInAppNotifier _notifier;
        private readonly ILogger<SendInAppNotificationConsumer> _logger;
        private readonly IBackgroundJobClient _jobClient;
        private readonly NotificationResolverHandler _resolverHandler;

        public SendInAppNotificationConsumer(
            IInAppNotifier notifier,
            ILogger<SendInAppNotificationConsumer> logger,
            IBackgroundJobClient jobClient,
            NotificationResolverHandler resolverHandler)
        {
            _notifier = notifier;
            _logger = logger;
            _jobClient = jobClient;
            _resolverHandler = resolverHandler;
        }

        public async Task Consume(ConsumeContext<SendInAppNotificationInternalCommand> context)
        {
            var msg = context.Message;

            try
            {
                var (_, _, _, _, userIds, subject, body, footer, lang) =
                await _resolverHandler.ResolveNotificationTemplatesAsync(msg.UnitId, msg.ModuleName, msg.EventTypeId);

                if (userIds == null || !userIds.Any())
                {
                    _logger.LogWarning("⚠️ No in-app users found to notify.");
                    await context.Publish(new SendInAppNotificationFailed
                    {
                        CorrelationId = msg.CorrelationId,
                        Reason = "No in-app user IDs resolved"
                    });
                    return;
                }

                var sent = await _notifier.SendInAppNotificationAsync(userIds, body, subject);

                if (sent)
                {
                    _logger.LogInformation("✅ In-app notification sent successfully.");
                    await context.Publish(new SendInAppNotificationCompleted
                    {
                        CorrelationId = msg.CorrelationId
                    });
                }
                else
                {
                    throw new Exception("In-app notification failed to send.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error sending in-app notification");

                // Retry using Hangfire after 5 minutes
                _jobClient.Schedule<INotificationHandler<SendInAppNotificationInternalCommand>>(h =>
                    h.ExecuteAsync(msg, msg.CorrelationId), TimeSpan.FromMinutes(5));

                await context.Publish(new SendInAppNotificationFailed
                {
                    CorrelationId = msg.CorrelationId,
                    Reason = ex.Message
                });
            }
        }
    }
}
