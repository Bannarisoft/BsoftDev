using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Events;
using Core.Domain.Entities;
using FAM.Infrastructure.Data;
using MassTransit;

namespace FAM.API.Consumers
{
    public class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly ILogger<UserCreatedEventConsumer> _logger;
        private readonly ApplicationDbContext _context;
        public UserCreatedEventConsumer(ILogger<UserCreatedEventConsumer> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            _logger.LogInformation($"Received UserCreatedEvent for UserId: {context.Message.UserId}");

            var asset = new AssetMasterGenerals
            {
                Id = 1,
                UserId = 1001,       // Hardcoded UserId
                AssetName = "Default Asset",
                CreatedDate = DateTime.UtcNow
                // Id = Guid.NewGuid(),
                // UserId = context.Message.UserId,
                // AssetName = "Default Asset",
                // CreatedDate = DateTime.UtcNow
            };

            _context.AssetMasterGenerals.Add(asset);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Fixed asset created for UserId: {context.Message.UserId}");
        }
    }
}