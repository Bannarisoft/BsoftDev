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
            try
            {
                var userEvent = context.Message;

                Log.Information("User Created: {UserName}, Email: {Email}, CorrelationId: {CorrelationId}",
                    userEvent.UserName, userEvent.Email, context.CorrelationId);

                // You can add additional logic here

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error consuming UserCreatedEvent");
                throw;
            }
            // var userEvent = context.Message;
            // Log.Information($"User Created: {userEvent.UserName}, CorrelationId: {userEvent.CorrelationId}");

            // await Task.CompletedTask;
        }
    }
}