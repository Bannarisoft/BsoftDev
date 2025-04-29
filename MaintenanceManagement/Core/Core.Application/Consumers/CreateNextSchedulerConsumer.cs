using Contracts.Commands;
using Contracts.Events.Maintenance;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Core.Application.Consumers
{
    public class CreateNextSchedulerConsumer : IConsumer<CreateNextSchedulerCommand>
    {
        private readonly IPreventiveSchedulerCommand _repository;
        private readonly ILogger<CreateNextSchedulerConsumer> _logger;

        public CreateNextSchedulerConsumer(IPreventiveSchedulerCommand repository, ILogger<CreateNextSchedulerConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CreateNextSchedulerCommand> context)
        {
            _logger.LogInformation("📩 Received CreateNextSchedulerCommand for WorkOrderId: {WorkOrderId}", context.Message.WorkOrderId);

            try
            {
                var created = await _repository.CreateNextSchedulerDetailAsync(context.Message.PreventiveSchedulerDetailId);

                if (!created)
                {
                    await context.Publish(new NextSchedulerCreationFailedEvent
                    {
                        CorrelationId = context.Message.CorrelationId,
                        WorkOrderId = context.Message.WorkOrderId,
                        Reason = "Failed to create scheduler in DB"
                    });

                    _logger.LogWarning("⚠️ Published failure event for WorkOrderId: {WorkOrderId}", context.Message.WorkOrderId);
                }
                else
                {
                    _logger.LogInformation("✅ Scheduler created in DB successfully for WorkOrderId: {WorkOrderId}", context.Message.WorkOrderId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception in CreateNextSchedulerConsumer for WorkOrderId: {WorkOrderId}", context.Message.WorkOrderId);

                await context.Publish(new NextSchedulerCreationFailedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    WorkOrderId = context.Message.WorkOrderId,
                    Reason = ex.Message
                });
            }
        }
    }
}