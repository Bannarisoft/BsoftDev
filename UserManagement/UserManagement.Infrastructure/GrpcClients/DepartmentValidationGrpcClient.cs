using System;
using System.Threading.Tasks;
using Contracts.Interfaces.External.IMaintenance;
using Grpc.Core;
using GrpcServices.Maintenance;
using Microsoft.AspNetCore.Http;

namespace UserManagement.Infrastructure.GrpcClients
{
    public class DepartmentValidationGrpcClient : IDepartmentValidationGrpcClient
    {
        private readonly DepartmentValidationService.DepartmentValidationServiceClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DepartmentValidationGrpcClient(
            DepartmentValidationService.DepartmentValidationServiceClient client,
            IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CheckIfDepartmentIsUsedAsync(int departmentId)
        {
            var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString()?.Trim();
            var metadata = new Metadata();

            if (!string.IsNullOrWhiteSpace(token))
            {
                if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    token = $"Bearer {token}";

                metadata.Add("Authorization", token);
                Console.WriteLine($"[gRPC] Sending token: {token}");
            }

            var request = new DepartmentUsageRequest { DepartmentId = departmentId };

            var response = await _client.IsDepartmentUsedInCostCenterAsync(request, new CallOptions(metadata));
            return response.IsUsed;
        }
    }
}
