using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Events.Users;
using MassTransit;
using SagaOrchestrator.Application.Orchestration.Interfaces.IAssets;

namespace SagaOrchestrator.Infrastructure.Services.AssetServices
{
    public class AssetSagaService
    {
        private readonly IAssetService _assetService;
        private readonly IPublishEndpoint _publishEndpoint;

        public AssetSagaService(IAssetService assetService, IPublishEndpoint publishEndpoint)
        {
            _assetService = assetService;
            _publishEndpoint = publishEndpoint;
        }

        public async Task TriggerAssetCreation(int userId, int assetId)
        {
            var asset = await _assetService.GetAssetByIdAsync(assetId);

            if (asset != null)
            {
                var assetCreatedEvent = new AssetCreatedEvent
                {
                    AssetId = asset.AssetId,
                    UserId = userId,
                    AssetName = asset.AssetName
                };

                await _publishEndpoint.Publish(assetCreatedEvent);
            }
        }
    }
}