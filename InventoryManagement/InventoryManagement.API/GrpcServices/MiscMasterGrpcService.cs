using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Domain.Common;
using Grpc.Core;
using Inventory.Grpc;

namespace InventoryManagement.API.GrpcServices
{
    public class MiscMasterGrpcService : MiscMasterService.MiscMasterServiceBase
    {
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        private readonly ILogger<MiscMasterGrpcService> _logger ;
        public MiscMasterGrpcService(IMiscMasterQueryRepository miscMasterQueryRepository, ILogger<MiscMasterGrpcService> logger)
        {
            _miscMasterQueryRepository = miscMasterQueryRepository;
            _logger = logger;
        }   
        public override async Task<MiscMastersListResponse> GetMiscMasterById( GetMiscMasterByIdRequest request, ServerCallContext context)
            {
                if (string.IsNullOrWhiteSpace(request.Misctype))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "misctype is required."));

                var key = request.Misctype.Trim().ToLowerInvariant();
                var typeKey = key switch
                {
                    "warehouse" or "warehousetype"              => Core.Domain.Common.MiscEnumEntity.WarehouseType, // "WarehouseType"
                    "storage"   or "storagetype"                => Core.Domain.Common.MiscEnumEntity.StorageType,
                    "area"      or "areatype"                   => Core.Domain.Common.MiscEnumEntity.AreaType,
                    "operation" or "operationtype"              => Core.Domain.Common.MiscEnumEntity.OperationType,
                    "floor"  or "Floor"                         => Core.Domain.Common.MiscEnumEntity.Floor,
                    "warehouseaisle" or "aisle"                 => Core.Domain.Common.MiscEnumEntity.WarehouseAisle,
                    "warehouseracklevel" or "racklevel"         => Core.Domain.Common.MiscEnumEntity.WarehouseRackLevel,
                    _ => throw new RpcException(new Status(StatusCode.InvalidArgument, $"Unsupported misctype: {request.Misctype}"))
                };

                var list = await _miscMasterQueryRepository.GetMiscMaster("", typeKey,"");

                var resp = new MiscMastersListResponse();
                resp.Items.AddRange(list.Select(d => new MiscMasterDto {
                    Id = d.Id, Code = d.Code, Description = d.Description, MiscTypeId = d.MiscTypeId
                }));
                return resp;
            }
        

    }
}