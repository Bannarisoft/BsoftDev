using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Events.Users;
using MassTransit;
using Serilog;

namespace UserManagement.Infrastructure.Consumers
{
    public class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
    {
        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var user = context.Message;
            Log.Information($"User Created: {user.UserName} with ID: {user.UserId}");
        }
    }
}