

using Contracts.Commands.Maintenance;
using Core.Application.Common.Interfaces.IWorkOrder;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Core.Application.Consumers
{
    public class RollbackWorkOrderConsumer : IConsumer<RollbackWorkOrderCommand>
    {
        private readonly IWorkOrderCommandRepository _workOrderRepo;
        private readonly ILogger<RollbackWorkOrderConsumer> _logger;

        public RollbackWorkOrderConsumer(IWorkOrderCommandRepository repo, ILogger<RollbackWorkOrderConsumer> logger)
        {
            _workOrderRepo = repo;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RollbackWorkOrderCommand> context)
        {
            _logger.LogWarning("⚠️ Rollback requested for WorkOrderId: {id} , CorrelationId: {cid},Reason: {reason}",
                context.Message.WorkOrderId, context.Message.CorrelationId, context.Message.Reason);

            // Rollback logic: for example, reset the WorkOrder status
            var result = await _workOrderRepo.RevertWorkOrderStatusAsync(context.Message.WorkOrderId);

            if (!result)
            {
                _logger.LogError("❌ Failed to revert work order status for ID: {id}", context.Message.WorkOrderId);
            }
        }
    }
}