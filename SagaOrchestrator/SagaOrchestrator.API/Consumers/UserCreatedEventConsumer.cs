
using Contracts.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SagaOrchestrator.API.Consumers
{
    public class UserCreatedEventConsumer : IConsumer<IUserCreated>
    {
        private readonly ILogger<UserCreatedEventConsumer> _logger;
        public UserCreatedEventConsumer(ILogger<UserCreatedEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IUserCreated> context)
        {
            _logger.LogInformation($"UserCreatedEvent received: {context.Message.UserId}");
            await Task.CompletedTask;
        }
    }
}