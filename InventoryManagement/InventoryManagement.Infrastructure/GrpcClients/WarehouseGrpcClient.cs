using Contracts.Dtos.Warehouse;
using Contracts.Interfaces.External.IWarehouse;
using Grpc.Core;
using Microsoft.AspNetCore.Http;

using WhGrpc = GrpcServices.Warehouse.Warehouse; 

namespace InventoryManagement.Infrastructure.GrpcClients
{
    public sealed class WarehouseGrpcClient : IWarehouseGrpcClient
    {
        private readonly WhGrpc.WarehouseService.WarehouseServiceClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WarehouseGrpcClient(WhGrpc.WarehouseService.WarehouseServiceClient client,
                                   IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResult<WarehouseDto>> GetAllAsync(int pageNumber, int pageSize, string? search, CancellationToken ct = default)
        {
            var req = new WhGrpc.WarehouseListRequest
            {
                PageNumber = pageNumber <= 0 ? 1 : pageNumber,
                PageSize   = pageSize   <= 0 ? 15 : pageSize,
                Search     = search ?? string.Empty
            };

            var metadata = BuildAuthMetadata();
            var res = await _client.GetAllWarehouseMasterAsync(req, headers: metadata, cancellationToken: ct);

            var items = res.Items.Select(x => new WarehouseDto
            {
                Id = x.Id,
                UnitId = x.UnitId,
                WarehouseCode = x.WarehouseCode ?? string.Empty,
                WarehouseName = x.WarehouseName ?? string.Empty,

                WarehouseTypeId = x.WarehouseTypeId,
                StorageTypeId   = x.StorageTypeId,
                AreaTypeId      = x.AreaTypeId,
                OperationTypeId = x.OperationTypeId,

                Capacity     = x.Capacity,
                CapacityUOMId = x.CapacityUOMId,

                CityId    = x.CityId,
                StateId   = x.StateId,
                CountryId = x.CountryId
                
            }).ToList();

            return new PagedResult<WarehouseDto>
            {
                Items      = items,
                TotalCount = res.TotalCount,
                PageNumber = res.PageNumber,
                PageSize   = res.PageSize
            };
        }

        public async Task<WarehouseDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var metadata = BuildAuthMetadata();
                var x = await _client.GetWarehouseByIdAsync(new WhGrpc.GetWarehouseByIdRequest { Id = id }, headers: metadata, cancellationToken: ct);

                return new WarehouseDto
                {
                    Id = x.Id,
                    UnitId = x.UnitId,
                    WarehouseCode = x.WarehouseCode ?? string.Empty,
                    WarehouseName = x.WarehouseName ?? string.Empty,

                    WarehouseTypeId = x.WarehouseTypeId,
                    StorageTypeId   = x.StorageTypeId,
                    AreaTypeId      = x.AreaTypeId,
                    OperationTypeId = x.OperationTypeId,

                    Capacity     = x.Capacity,
                    CapacityUOMId = x.CapacityUOMId,

                    CityId    = x.CityId,
                    StateId   = x.StateId,
                    CountryId = x.CountryId                    
                };
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                return null;
            }
        }

        private Metadata BuildAuthMetadata()
        {
            var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("Authorization token not found.");

            if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                token = $"Bearer {token}";

            return new Metadata { { "Authorization", token } };
        }
    }
}
