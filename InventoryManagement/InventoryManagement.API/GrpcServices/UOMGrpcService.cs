using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUOM;
using Grpc.Core;
using Inventory.Grpc;

namespace InventoryManagement.API.GrpcServices
{
    public class UOMGrpcService : InventoryUOMService.InventoryUOMServiceBase
    {
        private readonly IUOMQueryRepository _uOMQueryRepository;

        public UOMGrpcService(IUOMQueryRepository uOMQueryRepository)
        {
            _uOMQueryRepository = uOMQueryRepository;
        }

        public override async Task<UOMListResponse> GetAllUOMs(GetAllUOMsRequest request, ServerCallContext context)
        {

            var uomlist = await _uOMQueryRepository.GetUOMAsync();
            
            var   response = new UOMListResponse();
            response.Items.AddRange(uomlist.Select(u => new UOMDto
            {
                Id = u.Id,
                Code = u.Code,
                UomName = u.UOMName,
                UomTypeId = u.UOMTypeId,
                IsActive = u.IsActive == Core.Domain.Common.BaseEntity.Status.Active
            }));
            return response;
        }
        
        
    }
}