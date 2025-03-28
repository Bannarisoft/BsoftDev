using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Events.Users;
using MassTransit;

namespace FAM.Infrastructure.Consumers
{
    public class AssetCreatedEventConsumer : IConsumer<AssetCreatedEvent>
    {
        public async Task Consume(ConsumeContext<AssetCreatedEvent> context)
        {
            var assetEvent = context.Message;
            Console.WriteLine($"Asset Created: {assetEvent.AssetName} for UserId: {assetEvent.UserId}");

            // Notify SagaOrchestrator of completion
            await context.Publish(new SagaCompletedEvent
            {
                UserId = assetEvent.UserId,
                Status = "Completed"
            });
        }
    }
}