// Warehouse.Api/GrpcServices/RackGrpcService.cs
using Grpc.Core;
using MediatR;
using GrpcServices.Warehouse.Rack;
using Core.Application.RackMaster.Queries.GetAllRackMaster;
using Core.Application.RackMaster.Queries.GetRackMasterById;

namespace Warehouse.Api.GrpcServices
{
    public sealed class RackGrpcService : RackService.RackServiceBase
    {
        private readonly IMediator _mediator;
        public RackGrpcService(IMediator mediator) => _mediator = mediator;

        public override async Task<RackListResponse> GetAllRackMaster(RackListRequest request, ServerCallContext context)
        {
            var page = request.PageNumber <= 0 ? 1  : request.PageNumber;
            var size = request.PageSize   <= 0 ? 15 : request.PageSize;

            var result = await _mediator.Send(new GetAllRackMasterQuery
            {
                PageNumber = page,
                PageSize   = size,
                SearchTerm = string.IsNullOrWhiteSpace(request.Search) ? null : request.Search
            }, context.CancellationToken);

            var resp = new RackListResponse
            {
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize   = result.PageSize
            };

            foreach (var r in result.Data)
                resp.Items.Add(MapToProto(r));    // <-- typed, no dynamic

            return resp;
        }

        public override async Task<RackDto> GetRackById(GetRackByIdRequest request, ServerCallContext context)
        {
            var dto = await _mediator.Send(new GetRackMasterByIdQuery { Id = request.Id }, context.CancellationToken);
            if (dto == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Rack {request.Id} not found."));

            return MapToProto(dto);               // <-- typed, no dynamic
        }

        private static RackDto MapToProto(RackMasterDto r)
        {
            // Convert nullables safely; decimals -> double as proto expects
            return new RackDto
            {
                Id             = r.Id,
                WarehouseId    = r.WarehouseId,
                RackCode       = r.RackCode ?? string.Empty,
                RackName       = r.RackName ?? string.Empty,
                FloorId        = r.FloorId        ?? 0,
                AisleId        = r.AisleId        ?? 0,
                RackLevelId    = r.RackLevelId    ?? 0,
                MaxCapacity    = (double)(r.MaxCapacity ?? 0m),
                CapacityUOMId  = r.CapacityUOMId  ?? 0,
                RackWidth      = (double)(r.RackWidth   ?? 0m),
                RackHeight     = (double)(r.RackHeight  ?? 0m),
                DimensionUOMId = r.DimensionUOMId ?? 0
            };
        }
    }
}
