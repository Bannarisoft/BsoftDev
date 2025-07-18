using BackgroundService.Application.Interfaces.Notification;
using BackgroundService.Application.Notification;
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

        public SendSmsNotificationConsumer(
            ISmsSender smsSender,
            ILogger<SendSmsNotificationConsumer> logger,
            IBackgroundJobClient jobClient,
            NotificationResolverHandler resolverHandler)
        {
            _smsSender = smsSender;
            _logger = logger;
            _jobClient = jobClient;
            _resolverHandler = resolverHandler;
        }

        public async Task Consume(ConsumeContext<SendSmsNotificationInternalCommand> context)
        {
            
            var msg = context.Message;
            Console.WriteLine($"✅ SMS Consumer Received: ",msg); 
            try
            {
                var (_, _, _, smsNumbers, _, subject, body, footer, langCode) =
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
                
                var sent = await _smsSender.SendSmsAsync(new()
                {
                    mobileNumbers = smsNumbers,
                    message = messageText
                });

                if (sent)
                {
                    await context.Publish(new SendSmsNotificationCompleted
                    {
                        CorrelationId = msg.CorrelationId
                    });
                }
                else
                {
                    throw new Exception("SMS sending failed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ SMS Notification Failed for CorrelationId: {CorrelationId}", msg.CorrelationId);

                _jobClient.Schedule<INotificationHandler<SendSmsNotificationInternalCommand>>(
                    handler => handler.ExecuteAsync(context.Message, msg.CorrelationId),
                    TimeSpan.FromMinutes(5)
                );

                await context.Publish(new SendSmsNotificationFailed
                {
                    CorrelationId = msg.CorrelationId,
                    Reason = ex.Message
                });
            }
        }
    }
}
