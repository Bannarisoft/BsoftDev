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
    public class CountryValidationGrpcClient : IFixedAssetCountryValidationGrpcClient
    {
        public readonly FixedAssetCountryService.FixedAssetCountryServiceClient _countryServiceClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CountryValidationGrpcClient( FixedAssetCountryService.FixedAssetCountryServiceClient countryServiceClient, IHttpContextAccessor httpContextAccessor )
        {
            _countryServiceClient = countryServiceClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CheckIfCountryIsUsedForFixedAssetAsync(int countryId)
        {
             var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString()?.Trim();
            var metadata = new Metadata();

            if (!string.IsNullOrWhiteSpace(token))
            {
                if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    token = $"Bearer {token}";

                metadata.Add("Authorization", token);
            }

            var request = new GrpcServices.FixedAsset.CountryUsageRequest { CountryId = countryId };
            var response = await _countryServiceClient.IsCountryUsedAsync(request, new CallOptions(metadata));

            return response.IsUsed;
        }
    }
}