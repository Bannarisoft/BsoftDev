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
        public MiscMasterGrpcService(IMiscMasterQueryRepository miscMasterQueryRepository)
        {
            _miscMasterQueryRepository = miscMasterQueryRepository;
        }
       
       
       
       public override async Task<MiscMastersListResponse> GetMiscMasterById(GetMiscMasterByIdRequest request, ServerCallContext context)
        {
            var warehouseTypes = await _miscMasterQueryRepository.GetMiscMaster("", MiscEnumEntity.MiscTypes.WarehouseType);
            var storageTypes = await _miscMasterQueryRepository.GetMiscMaster("", MiscEnumEntity.MiscTypes.StorageType);

            var allTypes = warehouseTypes.Concat(storageTypes).ToList();

            var response = new MiscMastersListResponse();
            response.Items.AddRange(allTypes.Select(d => new MiscMasterDto
            {
                Id = d.Id,
                Code = d.Code,
                Description = d.Description,
                MiscTypeId = d.MiscTypeId
            }));

            return response;
        }

    //    public override async Task<MiscMastersListResponse> GetMiscMasterById(GetMiscMasterByIdRequest request, ServerCallContext context)
        //     {
        //         var inventorymiscmaster = await _miscMasterQueryRepository.GetMiscMaster("", "WarehouseType");

        //         var response = new MiscMastersListResponse();

        //        response.Items.AddRange(inventorymiscmaster.Select(d => new MiscMasterDto
        //        {
        //            Id = d.Id,
        //            Code = d.Code,
        //            Description = d.Description,
        //            MiscTypeId = d.MiscTypeId

        //         }));

        //         return response;
        //     } 
    }
}