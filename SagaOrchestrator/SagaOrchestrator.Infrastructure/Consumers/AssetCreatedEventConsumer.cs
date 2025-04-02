using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Events.Users;
using MassTransit;
using Serilog;

namespace SagaOrchestrator.Infrastructure.Consumers
{
    public class AssetCreatedEventConsumer : IConsumer<AssetCreatedEvent>
    {
        public async Task Consume(ConsumeContext<AssetCreatedEvent> context)
        {
            var assetEvent = context.Message;
            Log.Information($"Asset Created: {assetEvent.AssetName}");

            await context.Publish(new SagaCompletedEvent
            {
                CorrelationId = assetEvent.CorrelationId,
                UserId = assetEvent.UserId,
                Status = "Completed"
            });
        }
    }
}