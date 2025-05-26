using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Dtos.Maintenance;
using Contracts.Interfaces.External.IFixedAssetManagement;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServices.FixedAssetManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client.Extensibility;

namespace MaintenanceManagement.Infrastructure.GrpcClients
{
    public class AssetSpecificationGrpcClient : IAssetSpecificationGrpcClient
    {
        private readonly AssetSpecificationService.AssetSpecificationServiceClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AssetSpecificationGrpcClient(AssetSpecificationService.AssetSpecificationServiceClient client, IHttpContextAccessor httpContextAccessor)
            {
                _client = client;
                _httpContextAccessor = httpContextAccessor;
            }

        public async Task<List<Contracts.Dtos.Maintenance.AssetSpecificationDto>> GetAllAssetSpecificationAsync()
        {
            // ✅ Get token from current HTTP Context
            var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("No Authorization token found in the current context.");
            }
            //  ✅ Ensure it has "Bearer " prefix
            if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {

                token = $"Bearer {token}";
            }

            var metadata = new Metadata
                {
                    { "Authorization", token }
                };
            //  ✅ Attach Authorization header
            var callOptions = new CallOptions(metadata);

            var response = await _client.GetAllAssetSpecificationAsync(new Empty(), callOptions);
            var assetspecification = response.AssetSpecifications
               .Select(proto => new Contracts.Dtos.Maintenance.AssetSpecificationDto
               {
                   AssetId = proto.AssetId,
                   SpecificationName = proto.SpecificationName,
                   SpecificationValue = proto.SpecificationValue,
                   CapitalizationDate = proto.CapitalizationDate.ToDateTimeOffset()
               }).ToList();
            return assetspecification;
                    
                  
        }
    }
}