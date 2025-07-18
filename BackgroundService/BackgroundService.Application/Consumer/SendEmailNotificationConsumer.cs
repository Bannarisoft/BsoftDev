using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts.Events.Notifications.WorkOrder.Email;
using BackgroundService.Application.Notification;
using BackgroundService.Application.Interfaces.Notification;

namespace BackgroundService.Application.Consumers
{
    public class SendEmailNotificationConsumer : IConsumer<SendEmailNotificationInternalCommand>
    {
        private readonly NotificationResolverHandler _resolverHandler;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<SendEmailNotificationConsumer> _logger;

        public SendEmailNotificationConsumer(
            NotificationResolverHandler resolverHandler,
            IEmailSender emailSender,
            ILogger<SendEmailNotificationConsumer> logger)
        {
            _resolverHandler = resolverHandler;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SendEmailNotificationInternalCommand> context)
        {
            var message = context.Message;

            try
            {
                // Resolve all recipient emails and templates
                var (toEmails, ccEmails, bccEmails, _, _, subject, body, footer, langCode)
                    = await _resolverHandler.ResolveNotificationTemplatesAsync(
                        message.UnitId,
                        message.ModuleName,
                        message.EventTypeId
                    );

                if (!toEmails.Any())
                {
                    _logger.LogWarning("No email recipients found for CorrelationId: {CorrelationId}", message.CorrelationId);
                    await context.Publish(new SendEmailNotificationFailed
                    {
                        CorrelationId = message.CorrelationId,
                        Reason = "No email recipients found"
                    });
                    return;
                }

                var success = await _emailSender.SendEmailAsync(
                    toEmails,
                    subject,
                    body,
                    footer,
                    ccEmails,
                    bccEmails
                );

                if (success)
                {
                    await context.Publish(new SendEmailNotificationCompleted
                    {
                        CorrelationId = message.CorrelationId
                    });
                }
                else
                {
                    await context.Publish(new SendEmailNotificationFailed
                    {
                        CorrelationId = message.CorrelationId,
                        Reason = "Email sending failed"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email notification failed for CorrelationId: {CorrelationId}", message.CorrelationId);
                await context.Publish(new SendEmailNotificationFailed
                {
                    CorrelationId = message.CorrelationId,
                    Reason = $"Exception: {ex.Message}"
                });
            }
        }
    }
}
