
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
            var departmentGroupName = context.Message.UserId.ToString(); 
            try
            {
                var result = await _nextScheduleService.CreateNextSchedulerDetailAsync(context.Message.SchedulerId);
                if (result)
                {                    
                    await context.Publish(new NextSchedulerCreatedEvent
                    {
                        CorrelationId = context.Message.CorrelationId
                    });                                      
                    // await _hubContext.Clients.All.SendAsync("ReceiveMessage",
                    // $"Preventive Schedule created successfully: {headerId}");                    

                    var notification = new
                    {
                        Title = "Work Order Created",
                        Message = $"Preventive Schedule created  '{headerId}' ",
                        CreatedBy = context.Message.UserId,
                        Timestamp = DateTime.UtcNow
                    };
                    await _hubContext.Clients.Group(departmentGroupName)
                        .SendAsync("ReceiveMessage", notification);
                }
                else
                {                              

                    var notification = new
                    {
                        Title = "Work Order Created",
                        Message = $"Failed to create schedule: '{headerId}' ",
                        CreatedBy = context.Message.UserId,
                        Timestamp = DateTime.UtcNow
                    };
                    await _hubContext.Clients.Group(departmentGroupName)
                        .SendAsync("ReceiveMessage", notification);

                    await context.Publish(new NextSchedulerCreationFailedEvent
                    {
                        CorrelationId = context.Message.CorrelationId,
                        Reason = "Failed to create next schedule"
                    });
                }
            }
            catch (Exception ex)
            {               
                var notification = new
                    {
                        Title = "Work Order Created",
                        Message = $"Error while creating preventive schedule (ID: {headerId}): {ex.Message}",
                        CreatedBy = context.Message.UserId,
                        Timestamp = DateTime.UtcNow
                    };
                await _hubContext.Clients.Group(departmentGroupName)
                        .SendAsync("ReceiveMessage", notification);            

                await context.RespondAsync(new NextSchedulerCreationFailedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    Reason = $"Exception: {ex.Message}"
                });

            }
        }
    }
}