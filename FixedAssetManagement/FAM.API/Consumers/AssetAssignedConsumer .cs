using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Events;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using MassTransit;

namespace FAM.API.Consumers
{
    public class AssetAssignedConsumer : IConsumer<IAssetAssigned>
    {
        private readonly ILogger<AssetAssignedConsumer> _logger;

        public AssetAssignedConsumer(ILogger<AssetAssignedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IAssetAssigned> context)
        {
            var message = context.Message;

            _logger.LogInformation("IAssetAssigned Event received: UserId = {UserId}, AssetId = {AssetId}",
                message.UserId, message.AssetId);

            // Perform any additional logic you need here without saving to the database
            await Task.CompletedTask;
        }
    }
}