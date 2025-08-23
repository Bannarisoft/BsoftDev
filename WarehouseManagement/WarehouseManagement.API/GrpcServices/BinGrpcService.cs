using Grpc.Core;
using GrpcServices.Warehouse.Bin;
using Core.Application.BinMaster.Queries.GetAllBinMaster; // for BinMasterDto if you placed it there
using Core.Application.Common.Interfaces.IBinMaster;

public sealed class BinGrpcService : BinService.BinServiceBase
{
    private readonly IBinMasterQueryRepository _repo;
    public BinGrpcService(IBinMasterQueryRepository repo) => _repo = repo;

    public override async Task<BinListResponse> GetAllBinMaster(BinListRequest request, ServerCallContext context)
    {
        var page = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var size = request.PageSize   <= 0 ? 50 : request.PageSize;

        var (items, total) = await _repo.GetAllAsync(page, size, request.Search ?? string.Empty);

        var resp = new BinListResponse
        {
            TotalCount = total,
            PageNumber = page,
            PageSize   = size
        };

        foreach (var b in items)
        {
            resp.Items.Add(new BinDto
            {
                Id            = b.Id,
                BinCode       = b.BinCode ?? "",
                BinName       = b.BinName ?? "",
                WarehouseId   = b.WarehouseId,                
                WarehouseName = b.WarehouseName ?? "",
                RackId        = b.RackId ?? 0,
                RackCode      = b.RackCode ?? "",
                RackName      = b.RackName ?? "",
                BinCapacity   = (double)b.BinCapacity,
                CapacityUOMId = b.CapacityUOMId                
            });
        }

        return resp;
    }

    public override async Task<BinDto> GetBinById(GetBinByIdRequest request, ServerCallContext context)
    {
        var b = await _repo.GetByIdAsync(request.Id);
        if (b is null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Bin {request.Id} not found."));

        return new BinDto
        {
            Id            = b.Id,
            BinCode       = b.BinCode ?? "",
            BinName       = b.BinName ?? "",
            WarehouseId   = b.WarehouseId,            
            WarehouseName = b.WarehouseName ?? "",
            RackId        = b.RackId ?? 0,
            RackCode      = b.RackCode ?? "",
            RackName      = b.RackName ?? "",
            BinCapacity   = (double)b.BinCapacity,
            CapacityUOMId = b.CapacityUOMId            
        };
    }
}
