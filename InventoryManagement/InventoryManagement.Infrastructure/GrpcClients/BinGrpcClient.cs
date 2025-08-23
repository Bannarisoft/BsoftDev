using Contracts.Dtos.Warehouse;                    // Contracts.Dtos.Warehouse.BinDto
using Contracts.Interfaces.External.IWarehouse;   // IBinGrpcClient
using Grpc.Core;
using BinGrpc = GrpcServices.Warehouse.Bin;       // <-- alias proto namespace

namespace InventoryManagement.Infrastructure.GrpcClients
{
    public sealed class BinGrpcClient : IBinGrpcClient
    {
        private readonly BinGrpc.BinService.BinServiceClient _client;
        public BinGrpcClient(BinGrpc.BinService.BinServiceClient client) => _client = client;

        public Task<List<BinDto>> GetAllMasterAsync(int warehouseId, int? rackId = null, string search = null, bool onlyActive = true, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        /*    public async Task<List<BinDto>> GetAllMasterAsync(
               int warehouseId, int? rackId = null, string? search = null, bool onlyActive = true, CancellationToken ct = default)
           {
               var req = new BinGrpc.BinListRequest
               {
                   WarehouseId = warehouseId,
                   Search      = search ?? string.Empty,
                   OnlyActive  = onlyActive
               };
               if (rackId.HasValue && rackId.Value > 0)
                   req.RackId = rackId.Value;

               var res = await _client.GetAllMasterAsync(req, cancellationToken: ct);

               // map PROTO -> Contracts DTO (note: proto uses ...UomId casing)
               return res.Bins
                         .Where(b => !onlyActive || b.IsActive == 1)
                         .Select(b => new BinDto
                         {
                             Id            = b.Id,
                             WarehouseId   = b.WarehouseId,
                             RackId        = b.RackId,
                             BinCode       = b.BinCode,
                             BinName       = b.BinName,
                             BinCapacity   = b.BinCapacity,
                             CapacityUOMId = b.CapacityUOMId, // map Uom -> UOM
                             IsActive      = b.IsActive
                         })
                         .ToList();
           }

           public async Task<BinDto?> GetBinByIdAsync(int id, CancellationToken ct = default)
           {
               try
               {
                   var b = await _client.GetByIdAsync(new BinGrpc.GetByIdRequest { Id = id }, cancellationToken: ct);
                   return new BinDto
                   {
                       Id            = b.Id,
                       WarehouseId   = b.WarehouseId,
                       RackId        = b.RackId,
                       BinCode       = b.BinCode,
                       BinName       = b.BinName,
                       BinCapacity   = b.BinCapacity,
                       CapacityUOMId = b.CapacityUOMId,
                       IsActive      = b.IsActive
                   };
               }
               catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
               {
                   return null;
               }
           }
    */
        public Task<BinDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
