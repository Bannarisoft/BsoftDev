using Contracts.Events;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Domain.Entities;
using FAM.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
{
    private readonly IBus _bus;
    private readonly ILogger<UserCreatedConsumer> _logger;
    private readonly ApplicationDbContext _dbContext;
    public UserCreatedConsumer(IBus bus, ILogger<UserCreatedConsumer> logger, ApplicationDbContext dbContext)
    {
        _bus = bus;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var userEvent = context.Message;

        // _logger.LogInformation($"Received UserCreatedEvent: {userEvent.UserId} - {userEvent.Email}");

        // // Check if AssetMaster already exists for this UserId
        // var existingAsset = await _dbContext.AssetMasterGenerals.FirstOrDefaultAsync(a => a.UserId == userEvent.UserId);

        // if (existingAsset == null)
        // {
        //     // Create a new AssetMaster entry
        //     var newAsset = new AssetMasterGenerals
        //     {
        //         // AssetId = Guid.NewGuid(),  // Generate new Asset ID
        //         AssetName = "Default Asset", // Assign a default asset name
        //         UserId = userEvent.UserId
        //     };

        //     _dbContext.AssetMasterGenerals.Add(newAsset);
        //     await _dbContext.SaveChangesAsync();

        //     _logger.LogInformation($"New AssetMaster record created for UserId: {userEvent.UserId}");
        // }
        // else
        // {
        //     _logger.LogInformation($"AssetMaster already exists for UserId: {userEvent.UserId}");
        // }
        _logger.LogInformation("User Created Event Received for {UserId}", context.Message.UserId);

        await _bus.Publish<IAssetAssigned>(new
        {
            CorrelationId = context.CorrelationId ?? Guid.NewGuid(),
            UserId = context.Message.UserId,
            AssetId = Guid.NewGuid()
        });
    }
}
