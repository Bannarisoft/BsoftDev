    using Grpc.Core;
    using MediatR;
    using GrpcServices.Warehouse.Warehouse;
    using Core.Application.WarehouseMaster;
    using Core.Application.WarehouseMaster.GetWarehouseMasterById;
    using Core.Application.WarehouseMaster.GetAllWarehouseMaster;

    public sealed class WarehouseGrpcService : WarehouseService.WarehouseServiceBase
    {
        private readonly IMediator _mediator;
        public WarehouseGrpcService(IMediator mediator) => _mediator = mediator;

        public override async Task<WarehouseListResponse> GetAllWarehouseMaster(
            WarehouseListRequest request, ServerCallContext context)
        {
            var page = request.PageNumber <= 0 ? 1  : request.PageNumber;
            var size = request.PageSize   <= 0 ? 15 : request.PageSize;

            var result = await _mediator.Send(new GetAllWarehouseMastersQuery
            {
                PageNumber = page,
                PageSize   = size,
                SearchTerm = string.IsNullOrWhiteSpace(request.Search) ? null : request.Search
            }, context.CancellationToken);

            var resp = new WarehouseListResponse
            {
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize   = result.PageSize
            };

            foreach (var w in result.Data)
                resp.Items.Add(MapToProto(w));

            return resp;
        }

    public override async Task<WarehouseDto> GetWarehouseById(
            GetWarehouseByIdRequest request, ServerCallContext context)
        {
            // ApiResponseDTO<GetAll.WarehouseMasterDto>
            var result = await _mediator.Send(new GetWarehouseMasterByIdQuery { Id = request.Id },
                                            context.CancellationToken);

            if (!result.IsSuccess || result.Data is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Warehouse {request.Id} not found."));

            // pass the DTO, not the wrapper
            return MapToProto(result.Data);
        }
        // Map your Application DTO (WarehouseMasterDto) -> proto WarehouseDto
        private static WarehouseDto MapToProto(WarehouseMasterDto w) => new WarehouseDto
        {
            Id = w.Id,
            UnitId = w.UnitId,
            WarehouseCode = w.WarehouseCode ?? string.Empty,
            WarehouseName = w.WarehouseName ?? string.Empty,
            WarehouseTypeId = w.WarehouseTypeId,
            StorageTypeId   = w.StorageTypeId,
            AreaTypeId      = w.AreaTypeId,
            OperationTypeId = w.OperationTypeId,
            Capacity      = (double)w.MaxCapacity,   // your DTO uses MaxCapacity (decimal)
            CapacityUOMId = w.CapacityUOMId,
            CityId    = w.CityId,
            StateId   = w.StateId,
            CountryId = w.CountryId
        };
    }
