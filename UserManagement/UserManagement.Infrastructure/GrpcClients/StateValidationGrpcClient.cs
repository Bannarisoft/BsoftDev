using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Interfaces.External.IFixedAssetManagement;
using Grpc.Core;
using GrpcServices.FixedAsset;
using Microsoft.AspNetCore.Http;

namespace UserManagement.Infrastructure.GrpcClients
{
    public class StateValidationGrpcClient : IFixedAssetStateValidationGrpcClient
    {
        public readonly FixedAssetStateService.FixedAssetStateServiceClient _fixedAssetStateServiceClient;

        private readonly IHttpContextAccessor _httpContextAccessor;


        public StateValidationGrpcClient(FixedAssetStateService.FixedAssetStateServiceClient fixedAssetStateServiceClient, IHttpContextAccessor httpContextAccessor)
        {
            _fixedAssetStateServiceClient = fixedAssetStateServiceClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CheckIfStateIsUsedForFixedAssetAsync(int stateId)
        {
              var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString()?.Trim();
            var metadata = new Metadata();

            if (!string.IsNullOrWhiteSpace(token))
            {
                if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    token = $"Bearer {token}";

                metadata.Add("Authorization", token);
            }

            var request = new GrpcServices.FixedAsset.StateUsageRequest { StateId = stateId };
            var response = await _fixedAssetStateServiceClient.IsStateUsedAsync(request, new CallOptions(metadata));

            return response.IsUsed;
        }
    }
}