using BackgroundService.Application.Helpers;
using BackgroundService.Application.Interfaces.Notification;
using BackgroundService.Application.Notification;
using BackgroundService.Application.Notification.Common.Interfaces;
using BackgroundService.Domain.Entities.Notification;
using Contracts.Events.Notifications;
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
        private readonly IIPAddressService _ipAddressService;
        private readonly INotificationLogger _loggerNotification;     

        public SendInAppNotificationConsumer(
            IInAppNotifier notifier,
            ILogger<SendInAppNotificationConsumer> logger,
            IBackgroundJobClient jobClient,
            NotificationResolverHandler resolverHandler,INotificationLogger loggerNotification, IIPAddressService ipAddressService)
        {
            _notifier = notifier;
            _logger = logger;
            _jobClient = jobClient;
            _resolverHandler = resolverHandler;
            _loggerNotification = loggerNotification;
            _ipAddressService = ipAddressService;
        }

        public async Task Consume(ConsumeContext<SendInAppNotificationInternalCommand> context)
        {
            var msg = context.Message;
            try
            {
                var (_, _, _, _, userIds, subject, body, footer, lang,eventTypeId,eventRuleId, channelId) =
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
                var tokens = new Dictionary<string, string>
                {
                    { "Module", msg.ModuleName },
                    { "Code", msg.Code },
                    { "Name", msg.Name },
                    { "Date", msg.Date.ToString("dd-MMM-yyyy") }
                };
                string resolvedSubject = TemplateHelper.ReplaceTokens(subject, tokens);
                string resolvedBody = TemplateHelper.ReplaceTokens(body, tokens);

                var sent = await _notifier.SendInAppNotificationAsync(userIds, resolvedBody, resolvedSubject);

                if (sent)
                {
                    _logger.LogInformation("✅ In-app notification sent successfully.");

                    /* _logger.LogInformation("📤 resolvebody: {resolvebody}, resolvesubject: {resolvesubject}",
                       resolvedSubject,resolvedBody); */
                        
                    await _loggerNotification.LogAsync(new NotificationEventLog
                    {
                        NotificationLevelRuleId = eventRuleId ?? 0,
                        NotificationStatusId = sent ? (int)NotificationEnum.NotificationStatus.Success : (int)NotificationEnum.NotificationStatus.Failed,
                        ActionStatus = sent ? "Sent" : "Failed",
                        ChannelId = (int)NotificationEnum.NotificationChannel.InApp,
                        MessageText = resolvedBody,
                        Timestamp = DateTime.UtcNow,
                        CreatedBy = int.Parse(_ipAddressService.GetCurrentUserId()),
                        CreatedDate = DateTime.UtcNow,
                        CreatedByName = _ipAddressService.GetUserName(),
                        CreatedIP = _ipAddressService.GetSystemIPAddress()
                    });

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
