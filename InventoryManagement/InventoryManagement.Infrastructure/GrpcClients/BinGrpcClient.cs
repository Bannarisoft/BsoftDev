using Contracts.Dtos.Warehouse;
using Contracts.Interfaces.External.IWarehouse;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using BinGrpc = GrpcServices.Warehouse.Bin;

public sealed class BinGrpcClient : IBinGrpcClient
{
    private readonly BinGrpc.BinService.BinServiceClient _client;
    private readonly IHttpContextAccessor _http;

    public BinGrpcClient(BinGrpc.BinService.BinServiceClient client, IHttpContextAccessor http)
    {
        _client = client;
        _http   = http;
    }

    private Metadata BuildAuth()
    {
        var token = _http.HttpContext?.Request?.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(token)) return new Metadata();
        if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            token = $"Bearer {token}";
        return new Metadata { { "Authorization", token } };
    }

    public async Task<List<BinDto>> GetAllBinMasterAsync(
        int warehouseId, int? rackId = null, string? search = null, bool onlyActive = true, CancellationToken ct = default)
    {
        // Pull a “big” page and filter here (keeps your repo unchanged).
        // If your dataset is large, consider adding server-side filter later.
        var req = new BinGrpc.BinListRequest
        {
            PageNumber = 1,
            PageSize   = 1000,                     // adjust if needed
            Search     = search ?? string.Empty
        };

        var res = await _client.GetAllBinMasterAsync(req, headers: BuildAuth(), cancellationToken: ct);

        var list = res.Items
            .Where(b => b.WarehouseId == warehouseId)
            .Where(b => !rackId.HasValue || b.RackId == rackId.Value)            
            .Select(b => new BinDto
            {
                Id            = b.Id,
                BinCode       = b.BinCode,
                BinName       = b.BinName,
                WarehouseId   = b.WarehouseId,
                WarehouseCode = b.WarehouseCode,
                WarehouseName = b.WarehouseName,
                RackId        = b.RackId == 0 ? (int?)null : b.RackId,
                RackCode      = b.RackCode,
                RackName      = b.RackName,
                BinCapacity   = b.BinCapacity,
                CapacityUOMId = b.CapacityUOMId,                
            })
            .ToList();

        return list;
    }

    public async Task<BinDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var res = await _client.GetBinByIdAsync(new BinGrpc.GetBinByIdRequest { Id = id }, headers: BuildAuth(), cancellationToken: ct);
            return new BinDto
            {
                Id            = res.Id,
                BinCode       = res.BinCode,
                BinName       = res.BinName,
                WarehouseId   = res.WarehouseId,
                WarehouseName = res.WarehouseName,
                RackId        = res.RackId == 0 ? (int?)null : res.RackId,
                RackName      = res.RackName,
                BinCapacity   = res.BinCapacity,
                CapacityUOMId = res.CapacityUOMId                
            };
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }
}
