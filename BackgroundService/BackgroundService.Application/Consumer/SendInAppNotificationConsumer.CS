using BackgroundService.Application.Interfaces.Notification;
using BackgroundService.Application.Notification;
using Contracts.Events.Notifications.WorkOrder;
using Contracts.Events.Notifications.WorkOrder.InApp;
using Hangfire;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BackgroundService.Application.Consumers
{
    public class SendInAppNotificationConsumer : IConsumer<SendNotificationInternalCommand>
    {
        private readonly IInAppNotifier _inAppNotifier;
        private readonly ILogger<SendInAppNotificationConsumer> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly NotificationResolverHandler _resolverHandler;

        public SendInAppNotificationConsumer(
            IInAppNotifier inAppNotifier,
            ILogger<SendInAppNotificationConsumer> logger,
            IBackgroundJobClient backgroundJobClient,NotificationResolverHandler resolverHandler)
        {
            _inAppNotifier = inAppNotifier;
            _logger = logger;
            _backgroundJobClient = backgroundJobClient; _resolverHandler = resolverHandler;
        }

        public async Task Consume(ConsumeContext<SendNotificationInternalCommand> context)
        {
            var msg = context.Message;
            try
            {
                _logger.LogInformation("📨 Processing SendInAppNotificationCommand for CorrelationId: {CorrelationId}", msg.CorrelationId);

                var(_, _, _, _, inAppUserIds, subject, body, _, _) =
                         await _resolverHandler.ResolveEmailChannelAsync(msg.UnitId, msg.ModuleName, msg.EventTypeId);
                 if (inAppUserIds  == null || !inAppUserIds .Any())
                    {
                        _logger.LogWarning("❌ No UserID recipients found for CorrelationId: {CorrelationId}", msg.CorrelationId);

                        await context.Publish(new SendInAppNotificationFailed
                        {
                            CorrelationId = msg.CorrelationId,
                            Reason = "No InApp user recipients resolved"
                        });
                        return;
                    }
                /* var result = await _inAppNotifier.SendInAppNotificationAsync(
                    context.Message.UserIds,
                    context.Message.Message,
                    context.Message.Title
                ); */
                var success = await _inAppNotifier.SendInAppNotificationAsync(inAppUserIds, body,subject);

                if (success)
                {
                    _logger.LogInformation("✅ InApp sent successfully for CorrelationId: {CorrelationId}", msg.CorrelationId);

                    await context.Publish(new SendInAppNotificationCompleted
                    {
                        CorrelationId = context.Message.CorrelationId
                    });
                }
                else
                {
                    throw new Exception("In-app notification failed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "In-app notification failed for CorrelationId: {CorrelationId}", context.Message.CorrelationId);

                // ✅ Schedule retry using Hangfire after 5 minutes    
                 _backgroundJobClient.Schedule<INotificationHandler<SendNotificationInternalCommand>>(handler =>
                    handler.ExecuteAsync(context.Message, context.Message.CorrelationId),
                    TimeSpan.FromMinutes(5)); 
                await context.Publish(new SendInAppNotificationFailed
                {
                    CorrelationId = context.Message.CorrelationId,
                    Reason = ex.Message
                });
            }
        }
    }
}