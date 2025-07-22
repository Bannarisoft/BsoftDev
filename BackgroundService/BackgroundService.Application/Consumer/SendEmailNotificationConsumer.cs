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
                // Resolve all recipient emails and templates
                var (toEmails, ccEmails, bccEmails, _, _, subject, header, body, footer, langCode, eventTypeId, eventRuleId, channelId)
                    = await _resolverHandler.ResolveNotificationTemplatesAsync(
                        msg.UnitId,
                        msg.ModuleName,
                        msg.EventTypeId
                    );                
                if (!toEmails.Any())
                {
                    _logger.LogWarning("No email recipients found for CorrelationId: {CorrelationId}", msg.CorrelationId);
                    await context.Publish(new SendEmailNotificationFailed
                    {
                        CorrelationId = msg.CorrelationId,
                        Reason = "No email recipients found"
                    });
                    return;
                }
                Console.WriteLine($"🛑 triggered id: {context.Message.CorrelationId}");

                var tokens = new Dictionary<string, string>
                {
                    { "Module", msg.ModuleName },
                    { "param1", msg.param1 },
                    { "param2", msg.param2 },
                    { "param3", msg.param3.ToString("dd-MMM-yyyy") }
                };
                string resolvedSubject = TemplateHelper.ReplaceTokens(subject, tokens);
                string resolvedBody = TemplateHelper.ReplaceTokens(body, tokens);
                
                     var success = await _emailSender.SendEmailAsync(
                    toEmails,
                    resolvedSubject,
                    header,
                    resolvedBody,
                    footer,
                    ccEmails,
                    bccEmails, channelId ?? 0, eventRuleId ?? 0, eventTypeId ?? 0
                ); 
               if (success)
                {

                    //  _logger.LogInformation("📤 ChannelId: {ChannelId}, EventRuleId: {EventRuleId}, EventTypeId: {EventTypeId}",
                    //     channelId, (int)NotificationEnum.NotificationChannel.Email, eventTypeId);    


                    await context.Publish(new SendEmailNotificationCompleted
                    {
                        CorrelationId = msg.CorrelationId
                    });
                    int savedLogId = await _loggerNotification.LogAsync(new NotificationEventLog
                    {
                        NotificationLevelRuleId = eventRuleId ?? 0,
                        NotificationStatusId = success ? (int)NotificationEnum.NotificationStatus.Success : (int)NotificationEnum.NotificationStatus.Failed,
                        ReadStatusId = (int)NotificationEnum.NotificationReadStatus.Read,
                        SendTo = string.Join(",", toEmails),
                        ActionStatus = success ? "Sent" : "Failed",
                        ChannelId = (int)NotificationEnum.NotificationChannel.Email, //channelId ?? 0,
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
                    //  await context.Publish(new SendEmailNotificationFailed
                    //  {
                    //      CorrelationId = msg.CorrelationId,
                    //      Reason = "Email sending failed"
                    //  }); 
                    throw new Exception("Email notification failed to send.");
                } 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email notification failed for CorrelationId: {CorrelationId}", msg.CorrelationId);
                if (msg.RetryCount < MaxRetries)
                {
                    msg.RetryCount++;

                    _jobClient.Schedule<INotificationHandler<SendEmailNotificationInternalCommand>>(h =>
                        h.ExecuteAsync(msg, msg.CorrelationId), TimeSpan.FromMinutes(5));

                    _logger.LogWarning("🔁 Scheduled retry #{Retry} for Email notification", msg.RetryCount);
                }
                else
                {
                    _logger.LogError("⛔ Max retry attempts reached. Notification dropped.");

                    await context.Publish(new SendEmailNotificationFailed
                    {
                        CorrelationId = msg.CorrelationId,
                        Reason = $"Max retry attempts ({MaxRetries}) exceeded. Last error: {ex.Message}"
                    });
                }
                
                _jobClient.Schedule<INotificationHandler<SendEmailNotificationInternalCommand>>(h =>
                    h.ExecuteAsync(msg, msg.CorrelationId), TimeSpan.FromMinutes(5));

                await context.Publish(new SendEmailNotificationFailed
                {
                    CorrelationId = msg.CorrelationId,
                    Reason = $"Exception: {ex.Message}"
                });
            }
        }
    }
}
