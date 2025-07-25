using BackgroundService.Application.Helpers;
using BackgroundService.Application.Interfaces.Notification;
using BackgroundService.Application.Notification;
using BackgroundService.Application.Notification.Common.Interfaces;
using BackgroundService.Domain.Entities.Notification;
using Contracts.Events.Notifications;
using Contracts.Events.Notifications.WorkOrder.Sms;
using Hangfire;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BackgroundService.Application.Consumers
{
    public class SendSmsNotificationConsumer : IConsumer<SendSmsNotificationInternalCommand>
    {
        private readonly ISmsSender _smsSender;
        private readonly ILogger<SendSmsNotificationConsumer> _logger;
        private readonly IBackgroundJobClient _jobClient;
        private readonly NotificationResolverHandler _resolverHandler;
        private readonly IIPAddressService _ipAddressService;
        private readonly INotificationLogger _loggerNotification;
        private const int MaxRetries = 3; 
        private readonly ITimeZoneService _timeZoneService;

        public SendSmsNotificationConsumer(
            ISmsSender smsSender,
            ILogger<SendSmsNotificationConsumer> logger,
            IBackgroundJobClient jobClient,
            NotificationResolverHandler resolverHandler, INotificationLogger loggerNotification, IIPAddressService ipAddressService, ITimeZoneService timeZoneService)
        {
            _smsSender = smsSender;
            _logger = logger;
            _jobClient = jobClient;
            _resolverHandler = resolverHandler;
            _loggerNotification = loggerNotification;
            _ipAddressService = ipAddressService;
            _timeZoneService = timeZoneService;
        }

        public async Task Consume(ConsumeContext<SendSmsNotificationInternalCommand> context)
        {            
            var msg = context.Message;
            Console.WriteLine($"✅ SMS Consumer Received: ",msg); 
            try
            {
                var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
                var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId);
                var (_, _, _, smsNumbers, _, subject, header,body, footer, langCode,eventTypeId,eventRuleId, channelId) =
                    await _resolverHandler.ResolveNotificationTemplatesAsync(
                        msg.UnitId, msg.ModuleName, msg.EventTypeId);

                if (smsNumbers == null || !smsNumbers.Any())
                {
                    await context.Publish(new SendSmsNotificationFailed
                    {
                        CorrelationId = msg.CorrelationId,
                        Reason = "No mobile numbers found"
                    });
                    return;
                }

                var messageText = $"Dear {subject}, We received a request to reset your password. Use the verification code below to proceed:Code:{body}, This code is valid for {footer} minutes.";     
                var tokens = new Dictionary<string, string>
                {
                    { "Module", msg.ModuleName },
                    { "param1", msg.param1 },
                    { "param2", msg.param2 },
                    { "param3", msg.param3.ToString("dd-MMM-yyyy") }
                };
                
                string resolvedBody = TemplateHelper.ReplaceTokens(body, tokens);
                
                var sent = await _smsSender.SendSmsAsync(new()
                {
                    mobileNumbers = smsNumbers,
                    message = messageText//resolvedBody
                });

                if (sent)
                {
                    _logger.LogInformation("✅ In-app notification sent successfully.");

                    await context.Publish(new SendSmsNotificationCompleted
                    {
                        CorrelationId = msg.CorrelationId
                    });
                    int savedLogId = await _loggerNotification.LogAsync(new NotificationEventLog
                    {
                        NotificationLevelRuleId = eventRuleId ?? 0,
                        NotificationStatusId = sent ? (int)NotificationEnum.NotificationStatus.Success : (int)NotificationEnum.NotificationStatus.Failed,
                        ReadStatusId = (int)NotificationEnum.NotificationReadStatus.Unread,
                        SendTo =  string.Join(",", smsNumbers),
                        ActionStatus = sent ? "Sent" : "Failed",
                        ChannelId = (int)NotificationEnum.NotificationChannel.SMS,
                        MessageText = resolvedBody,
                        Timestamp = currentTime,
                        CreatedBy = int.Parse(_ipAddressService.GetCurrentUserId()),
                        CreatedDate = DateTime.UtcNow,
                        CreatedByName = _ipAddressService.GetUserName(),
                        CreatedIP = _ipAddressService.GetSystemIPAddress()
                    });
                }
                else
                {
                     await context.Publish(new SendSmsNotificationFailed
                    {
                        CorrelationId = msg.CorrelationId,
                        Reason = "Sms sending failed"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ SMS Notification Failed for CorrelationId: {CorrelationId}", msg.CorrelationId);
                if (msg.RetryCount < MaxRetries)
                {
                    msg.RetryCount++;

                    _jobClient.Schedule<INotificationHandler<SendSmsNotificationInternalCommand>>(h =>
                        h.ExecuteAsync(msg, msg.CorrelationId), TimeSpan.FromMinutes(5));

                    _logger.LogWarning("🔁 Scheduled retry #{Retry} for Sms notification", msg.RetryCount);
                }
                else
                {
                    _logger.LogError("⛔ Max retry attempts reached. Notification dropped.");

                    await context.Publish(new SendSmsNotificationFailed
                    {
                        CorrelationId = msg.CorrelationId,
                        Reason = $"Max retry attempts ({MaxRetries}) exceeded. Last error: {ex.Message}"
                    });
                }
                
                _jobClient.Schedule<INotificationHandler<SendSmsNotificationInternalCommand>>(h =>
                    h.ExecuteAsync(msg, msg.CorrelationId), TimeSpan.FromMinutes(5));

                await context.Publish(new SendSmsNotificationFailed
                {
                    CorrelationId = msg.CorrelationId,
                    Reason = ex.Message
                });
            }
        }
    }
}
