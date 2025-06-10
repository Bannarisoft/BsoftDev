
using Contracts.Commands.Maintenance;
using Contracts.Events.Maintenance;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Application.Common.RealTimeNotificationHub;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Core.Application.Consumers
{
    public class ScheduleNextPreventiveTaskConsumer : IConsumer<ScheduleNextPreventiveTaskCommand>
    {
        private readonly IPreventiveSchedulerCommand _nextScheduleService;
        private readonly IHubContext<WorkOrderScheduleHub> _hubContext;

        public ScheduleNextPreventiveTaskConsumer(IPreventiveSchedulerCommand nextScheduleService, IHubContext<WorkOrderScheduleHub> hubContext)
        {
            _nextScheduleService = nextScheduleService;
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<ScheduleNextPreventiveTaskCommand> context)
        {
            var headerId = context.Message.SchedulerId;
            try
            {
                var result = await _nextScheduleService.CreateNextSchedulerDetailAsync(context.Message.SchedulerId);
                if (result)
                {
                    await context.Publish(new NextSchedulerCreatedEvent
                    {
                        CorrelationId = context.Message.CorrelationId
                    });                                
                    //await _hubContext.Clients.Group(context.Message.SchedulerId.ToString())
                    //    .SendAsync("ReceiveMessage", $"✅ Schedule updated: {context.Message.SchedulerId}");
                    await _hubContext.Clients.All.SendAsync("ReceiveMessage",
                    $"Preventive Schedule created successfully: {headerId}");
                }
                else
                {                    
                   // await _hubContext.Clients.Group(context.Message.SchedulerId.ToString())
                     //        .SendAsync("ReceiveMessage", $"❌ Failed to create schedule: {context.Message.SchedulerId}");
                    await _hubContext.Clients.All.SendAsync("ReceiveMessage",
                    $"Preventive Schedule created successfully: {headerId}");

                    await context.Publish(new NextSchedulerCreationFailedEvent
                    {
                        CorrelationId = context.Message.CorrelationId,
                        Reason = "Failed to create next schedule"
                    });
                }
            }
            catch (Exception ex)
            {                
                //await _hubContext.Clients.Group(context.Message.SchedulerId.ToString())
                //    .SendAsync("ReceiveMessage", $"❌ Exception for schedule {context.Message.SchedulerId}: {ex.Message}");
               await _hubContext.Clients.All.SendAsync("ReceiveMessage",
                    $"❌ Error while creating preventive schedule (ID: {headerId}): {ex.Message}");

                await context.RespondAsync(new NextSchedulerCreationFailedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    Reason = $"Exception: {ex.Message}"
                });
            }
        }
    }
}