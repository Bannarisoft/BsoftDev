using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Interfaces.External.IFixedAssetManagement;
using GrpcServices.FixedAsset;
using Microsoft.AspNetCore.Http;
using Grpc.Core;

namespace UserManagement.Infrastructure.GrpcClients
{
    public class CityValidationGrpcClient : IFixedAssetCityValidationGrpcClient
    {

        private readonly FixedAssetCityService.FixedAssetCityServiceClient _client;
         private readonly IHttpContextAccessor _httpContextAccessor;

        public CityValidationGrpcClient(FixedAssetCityService.FixedAssetCityServiceClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
        }  
      
       public async Task<bool> CheckIfCityIsUsedForFixedAssetAsync(int cityId)
        {
            var request = new CityUsageRequest
            {
                CityId = cityId
            };

            var response = await _client.IsCityUsedAsync(request);
            return response.IsUsed;
        }  
        
    }
}