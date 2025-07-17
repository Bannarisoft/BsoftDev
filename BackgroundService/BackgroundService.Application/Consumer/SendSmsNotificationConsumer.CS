using MassTransit;
using Hangfire;
using Microsoft.Extensions.Logging;
using BackgroundService.Application.Interfaces.Notification;
using BackgroundService.Application.Notification;
using Contracts.Events.Notifications.WorkOrder.Sms;
using Contracts.Events.Notifications.Internal;
using Contracts.Events.Notifications.WorkOrder;

namespace BackgroundService.Application.Consumers
{
    public class SendSmsNotificationConsumer : IConsumer<SendNotificationInternalCommand>
    {
        private readonly ISmsSender _smsSender;
        private readonly ILogger<SendSmsNotificationConsumer> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly NotificationResolverHandler _resolverHandler;

        public SendSmsNotificationConsumer(
            ISmsSender smsSender,
            ILogger<SendSmsNotificationConsumer> logger,
            IBackgroundJobClient backgroundJobClient,
            NotificationResolverHandler resolverHandler)
        {
            _smsSender = smsSender;
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
            _resolverHandler = resolverHandler;
        }

        public async Task Consume(ConsumeContext<SendNotificationInternalCommand> context)
        {
            var msg = context.Message;
            try
            {
                _logger.LogInformation("📨 Processing SendSMSNotificationCommand for CorrelationId: {CorrelationId}", msg.CorrelationId);

                var (_, _, _,mobileNumbers,  _, _, body, _, _) =
                         await _resolverHandler.ResolveEmailChannelAsync(msg.UnitId, msg.ModuleName, msg.EventTypeId); 

                if (mobileNumbers  == null || !mobileNumbers .Any())
                    {
                        _logger.LogWarning("❌ No email recipients found for CorrelationId: {CorrelationId}", msg.CorrelationId);

                        await context.Publish(new SendSmsNotificationFailed
                        {
                            CorrelationId = msg.CorrelationId,
                            Reason = "No sms recipients resolved"
                        });
                        return;
                    }
                var success = await _smsSender.SendSmsAsync(mobileNumbers, body);

                if (success)
                {
                    _logger.LogInformation("✅ SMS sent successfully for CorrelationId: {CorrelationId}", msg.CorrelationId);
                    await context.Publish(new SendSmsNotificationCompleted
                    {
                        CorrelationId = context.Message.CorrelationId
                    });
                }
                else
                {
                    throw new Exception("SMS sending failed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMS sending failed for CorrelationId: {CorrelationId}", context.Message.CorrelationId);

                // Schedule retry via Hangfire
                     _backgroundJobClient.Schedule<INotificationHandler<SendNotificationInternalCommand>>(handler =>
                    handler.ExecuteAsync(context.Message, context.Message.CorrelationId),
                    TimeSpan.FromMinutes(5)); 

                await context.Publish(new SendSmsNotificationFailed
                {
                    CorrelationId = context.Message.CorrelationId,
                    Reason = ex.Message
                });
            }
        }
    }
}
