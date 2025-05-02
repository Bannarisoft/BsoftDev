
using Contracts.Commands.Maintenance;
using Contracts.Events.Maintenance;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using MassTransit;

namespace Core.Application.Consumers
{
    public class ScheduleNextPreventiveTaskConsumer : IConsumer<ScheduleNextPreventiveTaskCommand>
    {
        private readonly IPreventiveSchedulerCommand _nextScheduleService;

        public ScheduleNextPreventiveTaskConsumer(IPreventiveSchedulerCommand nextScheduleService)
        {
            _nextScheduleService = nextScheduleService;
        }

        public async Task Consume(ConsumeContext<ScheduleNextPreventiveTaskCommand> context)
        {
            try
            {
                var result = await _nextScheduleService.CreateNextSchedulerDetailAsync(context.Message.SchedulerId);
                if (result)
                {
                    await context.Publish(new NextSchedulerCreatedEvent
                    {
                        CorrelationId = context.Message.CorrelationId
                    });
                }
                else
                {
                    await context.Publish(new NextSchedulerCreationFailedEvent
                    {
                        CorrelationId = context.Message.CorrelationId,
                        Reason = "Failed to create next schedule"
                    });
                }
            }
            catch (Exception ex)
            {
                await context.RespondAsync(new NextSchedulerCreationFailedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    Reason = $"Exception: {ex.Message}"
                });
            }
        }
    }
}