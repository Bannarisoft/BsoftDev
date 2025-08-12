using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.Item.ItemCategory;
using Core.Application.Common.Interfaces.Item.ItemGroup;
using Grpc.Core;
using Inventory.Grpc;

namespace InventoryManagement.API.GrpcServices
{
    public class ItemGroupGrpcService : InventoryItemGroupService.InventoryItemGroupServiceBase
    {
        private readonly IItemGroupQueryRepository _itemCategoryQueryRepository;


        public ItemGroupGrpcService(IItemGroupQueryRepository itemGroupQueryRepository )
        {
            _itemCategoryQueryRepository = itemGroupQueryRepository;
        }
        
         public override async Task<ItemGroupListResponse> GetAllItemGroups( GetAllItemGroupsRequest request, ServerCallContext context)
        {
            // Get item groups from repository
            var itemGroups = await _itemCategoryQueryRepository.GetAllItemGroupsAsync();

            // Map to proto response
            var response = new ItemGroupListResponse();
            response.Items.AddRange(itemGroups.Select(g => new ItemGroupDto
            {
                Id = g.Id,
                UnitId = g.UnitId,
                ItemGroupCode = g.ItemGroupCode,
                ItemGroupName = g.ItemGroupName,
                IsActive = g.IsActive == Core.Domain.Common.BaseEntity.Status.Active

               
            }));

            return response;
        }
    }
}