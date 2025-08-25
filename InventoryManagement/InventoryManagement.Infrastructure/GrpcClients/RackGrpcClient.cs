using Contracts.Dtos.Warehouse;                    // your DTO: Contracts.Dtos.Warehouse.RackDto
using Contracts.Interfaces.External.IWarehouse;   // PagedResult<T>, IRackGrpcClient
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using RackGrpc = GrpcServices.Warehouse.Rack;     // <-- alias the proto namespace

namespace InventoryManagement.Infrastructure.GrpcClients
{
    public sealed class RackGrpcClient : IRackGrpcClient
    {
        private readonly RackGrpc.RackService.RackServiceClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public RackGrpcClient(RackGrpc.RackService.RackServiceClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor; // ✅ fixed
        }

        private Metadata GetAuthMetadata()
        {
            var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(token)) throw new UnauthorizedAccessException("Authorization token not found.");
            if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) token = $"Bearer {token}";
            return new Metadata { { "Authorization", token } };
        }
        public async Task<PagedResult<RackDto>> GetAllAsync(int pageNumber, int pageSize, string? search, CancellationToken ct = default)
        {
            var req = new RackGrpc.RackListRequest
            {
                PageNumber = pageNumber <= 0 ? 1 : pageNumber,
                PageSize = pageSize <= 0 ? 15 : pageSize,
                Search = search ?? string.Empty
            };

           

            var res = await _client.GetAllRackMasterAsync(req, headers: GetAuthMetadata(),  cancellationToken: ct);

            // Map PROTO -> Contracts DTO (be explicit about target type)
            var items = res.Items.Select(r => new RackDto
            {
                Id = r.Id,
                WarehouseId = r.WarehouseId,
                RackCode = r.RackCode,
                RackName = r.RackName,
                FloorId = r.FloorId,
                AisleId = r.AisleId,
                RackLevelId = r.RackLevelId,
                MaxCapacity = r.MaxCapacity,
                CapacityUOMId = r.CapacityUOMId,   // note proto casing: Uom
                RackWidth = r.RackWidth,
                RackHeight = r.RackHeight,
                DimensionUOMId = r.DimensionUOMId
            }).ToList();

            return new PagedResult<RackDto>
            {
                Items = items,
                TotalCount = res.TotalCount,
                PageNumber = res.PageNumber,
                PageSize = res.PageSize
            };
        }

        public async Task<RackDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var res = await _client.GetRackByIdAsync(new RackGrpc.GetRackByIdRequest { Id = id }, headers: GetAuthMetadata(), cancellationToken: ct);
                return new RackDto
                {
                    Id = res.Id,
                    WarehouseId = res.WarehouseId,
                    RackCode = res.RackCode,
                    RackName = res.RackName,
                    FloorId = res.FloorId,
                    AisleId = res.AisleId,
                    RackLevelId = res.RackLevelId,
                    MaxCapacity = res.MaxCapacity,
                    CapacityUOMId = res.CapacityUOMId,
                    RackWidth = res.RackWidth,
                    RackHeight = res.RackHeight,
                    DimensionUOMId = res.DimensionUOMId
                };
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                return null;
            }
        }
    }
}
