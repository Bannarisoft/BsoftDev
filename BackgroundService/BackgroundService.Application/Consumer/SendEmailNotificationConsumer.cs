using MassTransit;
using Microsoft.Extensions.Logging;
using Hangfire;
using BackgroundService.Application.Interfaces.Notification;
using BackgroundService.Application.Notification;
using Contracts.Events.Notifications.WorkOrder.Email;
using Contracts.Events.Notifications.Internal.Email;
using Contracts.Events.Notifications.WorkOrder;

namespace BackgroundService.Application.Consumers
{
    public class SendEmailNotificationConsumer : IConsumer<SendEmailNotificationInternalCommand>
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<SendEmailNotificationConsumer> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;        
        private readonly NotificationResolverHandler _resolverHandler;

        public SendEmailNotificationConsumer(
            IEmailSender emailSender,
            ILogger<SendEmailNotificationConsumer> logger,
            IBackgroundJobClient backgroundJobClient,            
            NotificationResolverHandler resolverHandler)
        {
            _emailSender = emailSender;
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;            
            _resolverHandler = resolverHandler;
        }

        public async Task Consume(ConsumeContext<SendEmailNotificationInternalCommand> context)
        {
            var msg = context.Message;
            try
            {
                _logger.LogInformation("📨 Processing SendEmailNotificationCommand for CorrelationId: {CorrelationId}", msg.CorrelationId);

                // ✅ Resolve targets and templates
                var (toEmails, ccEmails, bccEmails, _, _, subject, body, footer, _) =
                    await _resolverHandler.ResolveEmailChannelAsync(msg.UnitId, msg.ModuleName, msg.EventTypeId);

                if (toEmails == null || !toEmails.Any())
                {
                    _logger.LogWarning("❌ No email recipients found for CorrelationId: {CorrelationId}", msg.CorrelationId);

                    await context.Publish(new SendEmailNotificationFailed
                    {
                        CorrelationId = msg.CorrelationId,
                        Reason = "No email recipients resolved"
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
                    _logger.LogInformation("✅ Email sent successfully for CorrelationId: {CorrelationId}", msg.CorrelationId);

                    await context.Publish(new SendEmailNotificationCompleted
                    {
                        CorrelationId = msg.CorrelationId
                    });
                }
                else
                {
                    throw new Exception("Email sending failed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Email sending failed for CorrelationId: {CorrelationId}", msg.CorrelationId);

                // ✅ Retry via Hangfire after 5 minutes
                _backgroundJobClient.Schedule<INotificationHandler<SendEmailNotificationInternalCommand>>(handler =>
                    handler.ExecuteAsync(msg, msg.CorrelationId),
                    TimeSpan.FromMinutes(5));

                await context.Publish(new SendEmailNotificationFailed
                {
                    CorrelationId = msg.CorrelationId,
                    Reason = ex.Message
                });
            }
        }
    }
}
