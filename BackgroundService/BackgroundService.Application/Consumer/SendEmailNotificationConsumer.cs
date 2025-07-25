using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts.Events.Notifications.WorkOrder.Email;
using BackgroundService.Application.Notification;
using BackgroundService.Application.Interfaces.Notification;
using BackgroundService.Domain.Entities.Notification;
using Contracts.Events.Notifications;
using BackgroundService.Application.Notification.Common.Interfaces;
using BackgroundService.Application.Helpers;
using Hangfire;

namespace BackgroundService.Application.Consumers
{
    public class SendEmailNotificationConsumer : IConsumer<SendEmailNotificationInternalCommand>
    {
        private readonly NotificationResolverHandler _resolverHandler;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<SendEmailNotificationConsumer> _logger;   
        private readonly INotificationLogger _loggerNotification;        
        private readonly IIPAddressService _ipAddressService;
        private readonly IBackgroundJobClient _jobClient;
        private const int MaxRetries = 3;

        public SendEmailNotificationConsumer(
            NotificationResolverHandler resolverHandler,
            IEmailSender emailSender,
            ILogger<SendEmailNotificationConsumer> logger, INotificationLogger loggerNotification, IIPAddressService ipAddressService, IBackgroundJobClient jobClient)
        {
            _resolverHandler = resolverHandler;
            _emailSender = emailSender;
            _logger = logger;
            _loggerNotification = loggerNotification;
            _ipAddressService = ipAddressService;
            _jobClient = jobClient;
        }

       public async Task Consume(ConsumeContext<SendEmailNotificationInternalCommand> context)
        {
            var msg = context.Message;

            try
            {
                var (toEmails, ccEmails, bccEmails, _, _, subject, header, body, footer, langCode, eventTypeId, eventRuleId, channelId)
                    = await _resolverHandler.ResolveNotificationTemplatesAsync(
                        msg.UnitId, msg.ModuleName, msg.EventTypeId
                    );

                if (!toEmails.Any())
                {
                    _logger.LogWarning("❗ No email recipients found for CorrelationId: {CorrelationId}", msg.CorrelationId);
                    await context.Publish(new SendEmailNotificationFailed
                    {
                        CorrelationId = msg.CorrelationId,
                        Reason = "No recipients found"
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

                var resolvedSubject = TemplateHelper.ReplaceTokens(subject, tokens);
                var resolvedBody = TemplateHelper.ReplaceTokens(body, tokens);

                var success = await _emailSender.SendEmailAsync(
                    toEmails, resolvedSubject, header, resolvedBody, footer,
                    ccEmails, bccEmails, channelId ?? 0, eventRuleId ?? 0, eventTypeId ?? 0
                );

                if (success)
                {
                    await context.Publish(new SendEmailNotificationCompleted
                    {
                        CorrelationId = msg.CorrelationId
                    });

                    await _loggerNotification.LogAsync(new NotificationEventLog
                    {
                        NotificationLevelRuleId = eventRuleId ?? 0,
                        NotificationStatusId = (int)NotificationEnum.NotificationStatus.Success,
                        ReadStatusId = (int)NotificationEnum.NotificationReadStatus.Read,
                        SendTo = string.Join(",", toEmails),
                        ActionStatus = "Sent",
                        ChannelId = (int)NotificationEnum.NotificationChannel.Email,
                        MessageText = resolvedBody,
                        Timestamp = DateTime.UtcNow,
                        CreatedBy = int.Parse(_ipAddressService.GetCurrentUserId()),
                        CreatedDate = DateTime.UtcNow,
                        CreatedByName = _ipAddressService.GetUserName(),
                        CreatedIP = _ipAddressService.GetSystemIPAddress()
                    });
                }
                else
                {
                    throw new Exception("❌ Email sending failed (no exception thrown by sender).");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "📛 Email notification failed for CorrelationId: {CorrelationId}", msg.CorrelationId);

                if (msg.RetryCount < MaxRetries)
                {
                    msg.RetryCount++;

                    _jobClient.Schedule<INotificationHandler<SendEmailNotificationInternalCommand>>(
                        h => h.ExecuteAsync(msg, msg.CorrelationId),
                        TimeSpan.FromMinutes(5));

                    _logger.LogWarning("🔁 Scheduled retry #{Retry} for Email notification CorrelationId: {CorrelationId}",
                        msg.RetryCount, msg.CorrelationId);
                }
                else
                {
                    _logger.LogError("⛔ Max retries ({MaxRetries}) reached. Logging failure.", MaxRetries);

                    await _loggerNotification.LogAsync(new NotificationEventLog
                    {
                        NotificationLevelRuleId = msg.EventRuleId,
                        NotificationStatusId = (int)NotificationEnum.NotificationStatus.Failed,
                        ReadStatusId = (int)NotificationEnum.NotificationReadStatus.Unread,
                        SendTo = "Unknown or Failed",
                        ActionStatus = "Failed",
                        ChannelId = (int)NotificationEnum.NotificationChannel.Email,
                        MessageText = ex.Message,
                        Timestamp = DateTime.UtcNow,
                        CreatedBy =_ipAddressService.GetUserId(),
                        CreatedDate = DateTime.UtcNow,
                        CreatedByName = _ipAddressService.GetUserName(),
                        CreatedIP = _ipAddressService.GetSystemIPAddress()
                    });

                    await context.Publish(new SendEmailNotificationFailed
                    {
                        CorrelationId = msg.CorrelationId,
                        Reason = $"Retry limit exceeded. Last error: {ex.Message}"
                    });
                }
            }
        }

       
    }
}
