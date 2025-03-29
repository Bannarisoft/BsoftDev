using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Events.Users;
using MassTransit;
using SagaOrchestrator.Application.Orchestration.Interfaces.IAssets;
using SagaOrchestrator.Application.Orchestration.Interfaces.IUsers;

namespace SagaOrchestrator.Infrastructure.Services
{
    public class OrchestratorService
    {
        private readonly IUserService _userService;
        private readonly IAssetService _assetService;
        private readonly IPublishEndpoint _publishEndpoint;
        public OrchestratorService(IUserService userService, IAssetService assetService, IPublishEndpoint publishEndpoint)
        {
            _userService = userService;
            _assetService = assetService;
            _publishEndpoint = publishEndpoint;
        }
        public async Task TriggerUserCreation(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);

            if (user != null)
            {
                var userCreatedEvent = new UserCreatedEvent
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.Email
                };
                await _publishEndpoint.Publish(userCreatedEvent);
            }
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