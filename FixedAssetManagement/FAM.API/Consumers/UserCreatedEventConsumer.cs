using Contracts.Events;
using MassTransit;
using System;
using System.Threading.Tasks;

public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
{
    private readonly IBus _bus;
    private readonly ILogger<UserCreatedConsumer> _logger;
    public UserCreatedConsumer(IBus bus, ILogger<UserCreatedConsumer> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        _logger.LogInformation("User Created Event Received for {UserId}", context.Message.UserId);

        await _bus.Publish<IAssetAssigned>(new
        {
            CorrelationId = context.CorrelationId ?? Guid.NewGuid(),
            UserId = context.Message.UserId,
            AssetId = Guid.NewGuid()
        });
    }
}
