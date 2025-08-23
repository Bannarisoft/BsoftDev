using Contracts.Dtos.Party;
using Contracts.Interfaces.External.IParty;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServices.PartyManagement;
using Microsoft.AspNetCore.Http;

namespace InventoryManagement.Infrastructure.GrpcClients
{
    public class PartyGrpcClient : IPartyGrpcClient
    {
        private readonly PartyService.PartyServiceClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PartyGrpcClient(PartyService.PartyServiceClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor; 
        }

        public async Task<List<Contracts.Dtos.Party.PartyDto>> GetAllPartyMasterAsync()
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

            var response = await _client.GetAllPartyMasterAsync(new Empty(), new CallOptions(metadata));

            return response.Parties.Select(u => new Contracts.Dtos.Party.PartyDto
            {
                PartyId = u.PartyId,
                PartyCode = u.PartyCode,
                PartyName = u.PartyName,
                RegistrationTypeId = u.RegistrationTypeId,
                GSTNumber = u.GSTNumber,
                GSTStateCode = u.GSTStateCode,
                PAN = u.Pan,
                TAN = u.Tan,
                MSMENO = u.Msmeno,
                IsTDSApplicable = u.IsTDSApplicable,
                IsTCSApplicable = u.IsTCSApplicable,
                IsGstReverseCharge = u.IsGstReverseCharge,
                CreditDays = u.CreditDays,
                PartyStatus = u.PartyStatus,
            }).ToList();
        }        
    }
}
