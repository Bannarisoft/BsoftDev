 using Grpc.Core;
using Microsoft.AspNetCore.Http;
 using Contracts.Interfaces.External.IUser;
using GrpcServices.UserManagement;
using Google.Protobuf.WellKnownTypes;

namespace MaintenanceManagement.Infrastructure.GrpcClients
{
    public class UnitGrpcClient : IUnitGrpcClient
        {
            private readonly UnitService.UnitServiceClient _client;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public UnitGrpcClient(UnitService.UnitServiceClient client, IHttpContextAccessor httpContextAccessor)
            {
                _client = client;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<List<Contracts.Dtos.Maintenance.UnitDto>> GetUnitAutoCompleteAsync()
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

                var response = await _client.GetAllUnitAsync(new Empty(), callOptions);

                var units = response.Units
                    .Select(proto => new Contracts.Dtos.Maintenance.UnitDto
                    {
                        UnitId = proto.UnitId,
                        UnitName = proto.UnitName,
                        ShortName = proto.ShortName,
                        UnitHeadName = proto.UnitHeadName,
                        OldUnitId=proto.OldUnitId                                         
                    }).ToList();

                return units;
            }
      
    }
 }
