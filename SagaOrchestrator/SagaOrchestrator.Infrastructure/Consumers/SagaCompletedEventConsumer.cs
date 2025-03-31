using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Events.Users;
using MassTransit;
using Serilog;

namespace SagaOrchestrator.Infrastructure.Consumers
{
    public class SagaCompletedEventConsumer : IConsumer<SagaCompletedEvent>
    {
        public async Task Consume(ConsumeContext<SagaCompletedEvent> context)
        {
            var sagaCompletedEvent = context.Message;
            Log.Information($"Saga Completed for UserId: {sagaCompletedEvent.UserId} with Status: {sagaCompletedEvent.Status}");

            await Task.CompletedTask;
        }
    }
}