using Contracts.Events;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Domain.Entities;
using FAM.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

public class UserCreatedConsumer : IConsumer<IUserCreatedEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<UserCreatedConsumer> _logger;
    public UserCreatedConsumer(IPublishEndpoint publishEndpoint, ILogger<UserCreatedConsumer> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IUserCreatedEvent> context)
    {
        var assetAssignedEvent = new AssetAssigned
        {
            CorrelationId = context.Message.CorrelationId,
            UserId = context.Message.UserId,
            AssetId = GenerateAssetId(context.Message.UserId)
        };

        await _publishEndpoint.Publish<IAssetAssigned>(assetAssignedEvent);

        _logger.LogInformation($"AssetAssigned event published for UserId: {context.Message.UserId}");
    }

    private int GenerateAssetId(int userId)
    {
        return userId + 1000;  // Mocked asset ID generation
    }


}
