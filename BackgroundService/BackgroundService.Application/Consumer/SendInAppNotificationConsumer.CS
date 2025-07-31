using BackgroundService.Application.DTO;
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
        private const int MaxRetries = 3; 

        public SendInAppNotificationConsumer(
            IInAppNotifier notifier,
            ILogger<SendInAppNotificationConsumer> logger,
            IBackgroundJobClient jobClient,
            NotificationResolverHandler resolverHandler, INotificationLogger loggerNotification, IIPAddressService ipAddressService)
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
                var (_, _, _, _, userIds, subject, header,body, footer, lang,eventTypeId,eventRuleId, channelId) =
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
                    { "param1", msg.param1 },
                    { "param2", msg.param2 },
                    { "param3", msg.param3.ToString("dd-MMM-yyyy") }
                };
                string resolvedSubject = TemplateHelper.ReplaceTokens(subject, tokens);
                string resolvedBody = TemplateHelper.ReplaceTokens(body, tokens);

                var contextData = new NotificationContext
                {
                    EventRuleId = eventRuleId ?? 0,
                    EventTypeId = eventTypeId ?? 0,
                    ChannelId = (int)NotificationEnum.NotificationChannel.InApp,
                    CreatedById = int.Parse(_ipAddressService.GetCurrentUserId()),
                    CreatedByName = _ipAddressService.GetUserName(),
                    CreatedIp = _ipAddressService.GetSystemIPAddress()
                };

                var sent = await _notifier.SendInAppNotificationAsync(userIds, resolvedBody, msg.ModuleName ,contextData);

                if (sent)
                {
                    _logger.LogInformation("✅ In-app notification sent successfully.");

                    /* _logger.LogInformation("📤 resolvebody: {resolvebody}, resolvesubject: {resolvesubject}",
                       resolvedSubject,resolvedBody); */
                        
                   /*  await _loggerNotification.LogAsync(new NotificationEventLog
                    {
                        NotificationLevelRuleId = eventRuleId ?? 0,
                        NotificationStatusId = sent ? (int)NotificationEnum.NotificationStatus.Success : (int)NotificationEnum.NotificationStatus.Failed,
                        ReadStatusId = (int)NotificationEnum.NotificationReadStatus.Unread,
                        ActionStatus = sent ? "Sent" : "Failed",
                        ChannelId = (int)NotificationEnum.NotificationChannel.InApp,
                        MessageText = resolvedBody,
                        Timestamp = DateTime.UtcNow,
                        CreatedBy = int.Parse(_ipAddressService.GetCurrentUserId()),
                        CreatedDate = DateTime.UtcNow,
                        CreatedByName = _ipAddressService.GetUserName(),
                        CreatedIP = _ipAddressService.GetSystemIPAddress()
                    }); */

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
                if (msg.RetryCount < MaxRetries)
                {
                    msg.RetryCount++;

                    _jobClient.Schedule<INotificationHandler<SendInAppNotificationInternalCommand>>(h =>
                        h.ExecuteAsync(msg, msg.CorrelationId), TimeSpan.FromMinutes(5));

                    _logger.LogWarning("🔁 Scheduled retry #{Retry} for InApp notification", msg.RetryCount);
                }
                else
                {
                    _logger.LogError("⛔ Max retry attempts reached. Notification dropped.");

                    await context.Publish(new SendInAppNotificationFailed
                    {
                        CorrelationId = msg.CorrelationId,
                        Reason = $"Max retry attempts ({MaxRetries}) exceeded. Last error: {ex.Message}"
                    });
                }
                
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
