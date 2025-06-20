using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Interfaces.External.IFixedAssetManagement;
using Grpc.Core;
using GrpcServices.FixedAsset;
using GrpcServices.Maintenance;
using Microsoft.AspNetCore.Http;


namespace UserManagement.Infrastructure.GrpcClients
{
    public class FixedAssetDepartmentValidationGrpcClient : IFixedAssetDepartmentValidationGrpcClient
    {

       public readonly FixedAssetDepartmentValidationService.FixedAssetDepartmentValidationServiceClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FixedAssetDepartmentValidationGrpcClient(FixedAssetDepartmentValidationService.FixedAssetDepartmentValidationServiceClient client, IHttpContextAccessor httpContextAccessor)
        {
             _client = client;
            _httpContextAccessor = httpContextAccessor;
        }
            public async Task<bool> CheckIfDepartmentIsUsedForFixedAssetAsync(int departmentId)
        {
            var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString()?.Trim();
            var metadata = new Metadata();

            if (!string.IsNullOrWhiteSpace(token))
            {
                if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    token = $"Bearer {token}";

                metadata.Add("Authorization", token);
            }

            var request = new GrpcServices.FixedAsset.DepartmentUsageRequest { DepartmentId = departmentId };
            var response = await _client.IsDepartmentUsedAsync(request, new CallOptions(metadata));

            return false;
        }
    }
}