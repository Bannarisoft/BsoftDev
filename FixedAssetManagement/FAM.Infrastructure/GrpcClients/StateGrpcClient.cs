using Contracts.Interfaces.External.IUser;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServices.UserManagement;
using Microsoft.AspNetCore.Http;


namespace FAM.Infrastructure.GrpcClients
{
    public class StateGrpcClient  : IStatesGrpcClient
    {
        private readonly StateService.StateServiceClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StateGrpcClient(StateService.StateServiceClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor; // ✅ fixed
        }

        public async Task<List<Contracts.Dtos.Users.StatesDto>> GetAllStateAsync()
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

            var response = await _client.GetAllStatesAsync(new Empty(), new CallOptions(metadata));

            return response.States.Select(u => new Contracts.Dtos.Users.StatesDto
            {
                StateId = u.StateId,
                StateName = u.StateName,
                StateCode = u.StateCode,
                CountryId = u.CountryId
            }).ToList();
        }
    }
}
