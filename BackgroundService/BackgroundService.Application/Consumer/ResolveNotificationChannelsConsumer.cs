using BackgroundService.Application.Notification;
using Contracts.Events.Notifications.WorkOrder;
using Contracts.Events.Notifications.WorkOrder.Email;
using Contracts.Events.Notifications.WorkOrder.InApp;
using Contracts.Events.Notifications.WorkOrder.Sms;
using MassTransit;

namespace BackgroundService.Application.Consumers
{
    public class ResolveNotificationChannelsConsumer : IConsumer<SendNotificationInternalCommand>
    {
        private readonly NotificationResolverHandler _resolverHandler;

        public ResolveNotificationChannelsConsumer(NotificationResolverHandler resolverHandler)
        {
            _resolverHandler = resolverHandler;
        }

       public async Task Consume(ConsumeContext<SendNotificationInternalCommand> context)
        {
            var channels = await _resolverHandler.ResolveNotificationChannelsAsync(
                context.Message.UnitId,
                context.Message.ModuleName,
                context.Message.EventTypeId);
            Console.WriteLine("Resolve consumer");
            Console.WriteLine("🔥 Channels from SQL:",channels);
            foreach (var c in channels)
                Console.WriteLine($" - {c}");
            //Console.WriteLine("🛑 Resolving channels for: {0}", context.Message.CorrelationId);
            Console.WriteLine($"🛑 Resolving channels for: {context.Message.CorrelationId}");

            // Publish internal notification commands
            if (channels.Contains("Email", StringComparer.OrdinalIgnoreCase))
            {
                await context.Publish(new SendEmailNotificationInternalCommand
                {
                    CorrelationId = context.Message.CorrelationId,
                    UnitId = context.Message.UnitId,
                    EventTypeId = context.Message.EventTypeId,
                    ModuleName = context.Message.ModuleName,
                    CreatedByName = context.Message.CreatedByName,
                    ChannelId = context.Message.ChannelId,
                    EventRuleId = context.Message.EventRuleId,
                    Code = context.Message.Code,
                    Name = context.Message.Name,
                    Date = context.Message.Date
                });
            }

            if (channels.Contains("SMS", StringComparer.OrdinalIgnoreCase))
            {
                await context.Publish(new SendSmsNotificationInternalCommand
                {
                    CorrelationId = context.Message.CorrelationId,
                    UnitId = context.Message.UnitId,
                    EventTypeId = context.Message.EventTypeId,
                    ModuleName = context.Message.ModuleName,
                    CreatedByName = context.Message.CreatedByName,                    
                    ChannelId= context.Message.ChannelId,
                    EventRuleId= context.Message.EventRuleId,
                    Code = context.Message.Code,
                    Name = context.Message.Name,
                    Date = context.Message.Date
                });
            }

            if (channels.Contains("InApp", StringComparer.OrdinalIgnoreCase))
            {
                await context.Publish(new SendInAppNotificationInternalCommand
                {
                    CorrelationId = context.Message.CorrelationId,
                    UnitId = context.Message.UnitId,
                    EventTypeId = context.Message.EventTypeId,
                    ModuleName = context.Message.ModuleName,
                    CreatedByName = context.Message.CreatedByName,                    
                    ChannelId= context.Message.ChannelId,
                    EventRuleId= context.Message.EventRuleId,
                    Code = context.Message.Code,
                    Name = context.Message.Name,
                    Date = context.Message.Date
                });
            }
            
            // ✅ Respond back to the saga
            await context.RespondAsync(new ResolveNotificationChannelsResponse
            {
                CorrelationId = context.Message.CorrelationId,
                Channels = channels.ToList()
            });
        }

    }
}
