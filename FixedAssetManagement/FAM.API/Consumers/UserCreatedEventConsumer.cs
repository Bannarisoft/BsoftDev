using System;
using System.Threading.Tasks;
using Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FAM.API.Consumers
{
    public class UserCreatedEventConsumer : IConsumer<IUserCreated>
    {
        private readonly ILogger<UserCreatedEventConsumer> _logger;
        // private readonly ApplicationDbContext _context;

        public UserCreatedEventConsumer(ILogger<UserCreatedEventConsumer> logger)
        {
            _logger = logger;
            // _context = context;
        }

        public async Task Consume(ConsumeContext<IUserCreated> context)
        {
            // _logger.LogInformation($"Allocating asset to user {context.Message.UserName}");

            // await context.Publish(new FixedAssetAllocatedEvent(Guid.NewGuid(), context.Message.UserId, "Laptop"));

            // _logger.LogInformation($"Received UserCreatedEvent for UserId: {context.Message.UserId}");

            // var asset = new AssetMasterGenerals
            // {
            //     Id = 1,
            //     UserId = 1001,       // Hardcoded UserId
            //     AssetName = "Default Asset",
            //     CreatedDate = DateTime.UtcNow
            //     // Id = Guid.NewGuid(),
            //     // UserId = context.Message.UserId,
            //     // AssetName = "Default Asset",
            //     // CreatedDate = DateTime.UtcNow
            // };

            // _context.AssetMasterGenerals.Add(asset);
            // await _context.SaveChangesAsync();

            // _logger.LogInformation($"Fixed asset created for UserId: {context.Message.UserId}");
        }
    }
}
