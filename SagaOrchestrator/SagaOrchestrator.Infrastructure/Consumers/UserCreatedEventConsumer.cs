using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Events.Users;
using MassTransit;
using Serilog;

namespace SagaOrchestrator.Infrastructure.Consumers
{
    public class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
    {
        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var userEvent = context.Message;
            Log.Information($"User Created: {userEvent.UserName}, CorrelationId: {userEvent.CorrelationId}");

            await Task.CompletedTask;
        }
    }
}