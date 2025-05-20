
using Contracts.Interfaces.External.IUser;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServices.UserManagement;
using Microsoft.AspNetCore.Http;

namespace FAM.Infrastructure.GrpcClients
{
    public class UnitGrpcClient : IUnitGrpcClient
    {
        private readonly UnitService.UnitServiceClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UnitGrpcClient(UnitService.UnitServiceClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor; // ✅ fixed
        }

        public async Task<List<Contracts.Dtos.Maintenance.UnitDto>> GetAllUnitAsync()
        {
            var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("Authorization token not found.");

            if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                token = $"Bearer {token}";

            var metadata = new Metadata
            {
                { "Authorization", token }
            };

            var response = await _client.GetAllUnitAsync(new Empty(), new CallOptions(metadata));

            return response.Units.Select(u => new Contracts.Dtos.Maintenance.UnitDto
            {
                UnitId = u.UnitId,
                UnitName = u.UnitName,
                ShortName = u.ShortName,
                UnitHeadName = u.UnitHeadName
            }).ToList();
        }
    }
}
