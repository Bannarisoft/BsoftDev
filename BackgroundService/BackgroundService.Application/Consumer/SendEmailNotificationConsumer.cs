using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts.Events.Notifications.WorkOrder.Email;
using BackgroundService.Application.Notification;
using BackgroundService.Application.Interfaces.Notification;
using BackgroundService.Domain.Entities.Notification;
using Contracts.Events.Notifications;
using BackgroundService.Application.Notification.Common.Interfaces;
using BackgroundService.Application.Helpers;

namespace BackgroundService.Application.Consumers
{
    public class SendEmailNotificationConsumer : IConsumer<SendEmailNotificationInternalCommand>
    {
        private readonly NotificationResolverHandler _resolverHandler;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<SendEmailNotificationConsumer> _logger;   
        private readonly INotificationLogger _loggerNotification;        
        private readonly IIPAddressService _ipAddressService;

        public SendEmailNotificationConsumer(
            NotificationResolverHandler resolverHandler,
            IEmailSender emailSender,
            ILogger<SendEmailNotificationConsumer> logger, INotificationLogger loggerNotification, IIPAddressService ipAddressService)
        {
            _resolverHandler = resolverHandler;
            _emailSender = emailSender;
            _logger = logger;
            _loggerNotification = loggerNotification;
            _ipAddressService = ipAddressService;  
        }

        public async Task Consume(ConsumeContext<SendEmailNotificationInternalCommand> context)
        {
            var msg = context.Message;

            try
            {
                // Resolve all recipient emails and templates
                var (toEmails, ccEmails, bccEmails, _, _, subject, body, footer, langCode,eventTypeId,eventRuleId, channelId)
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
                    { "Code", msg.Code },
                    { "Name", msg.Name },
                    { "Date", msg.Date.ToString("dd-MMM-yyyy") }
                };
                string resolvedSubject = TemplateHelper.ReplaceTokens(subject, tokens);
                string resolvedBody = TemplateHelper.ReplaceTokens(body, tokens);

                var success = await _emailSender.SendEmailAsync(
                    toEmails,
                    resolvedSubject,
                    resolvedBody,
                    footer,
                    ccEmails,
                    bccEmails,channelId??0,eventRuleId??0, eventTypeId??0
                );

                if (success)
                {
                    _logger.LogInformation("✅ Email notification sent successfully.");                
                    /*  _logger.LogInformation("📤 ChannelId: {ChannelId}, EventRuleId: {EventRuleId}, EventTypeId: {EventTypeId}",
                        channelId, (int)NotificationEnum.NotificationChannel.Email, eventTypeId);                    
                    _logger.LogInformation("message: {message}", resolvedBody);  */
                    
                    await context.Publish(new SendEmailNotificationCompleted
                    {
                        CorrelationId = msg.CorrelationId
                    });
                    await _loggerNotification.LogAsync(new NotificationEventLog
                    {
                        NotificationLevelRuleId = eventRuleId ?? 0,
                        NotificationStatusId = success ? (int)NotificationEnum.NotificationStatus.Success : (int)NotificationEnum.NotificationStatus.Failed,
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
                    await context.Publish(new SendEmailNotificationFailed
                    {
                        CorrelationId = msg.CorrelationId,
                        Reason = "Email sending failed"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email notification failed for CorrelationId: {CorrelationId}", msg.CorrelationId);
                await context.Publish(new SendEmailNotificationFailed
                {
                    CorrelationId = msg.CorrelationId,
                    Reason = $"Exception: {ex.Message}"
                });
            }
        }
    }
}
