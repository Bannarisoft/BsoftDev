using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Dtos.Inventory;
using Contracts.Interfaces.External.IInvetoryManagement;
using Grpc.Core;
using Inventory.Grpc;
using Microsoft.AspNetCore.Http;


namespace WarehouseManagement.Infrastructure.GrpcClients
{
    public class InventoryItemGroupGrpcClient : IItemGroupGrpcClient
    {
        private readonly InventoryItemGroupService.InventoryItemGroupServiceClient _grpcClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public async Task<List<Contracts.Dtos.Inventory.ItemGroupDto>> GetAllItemGroupsAsync()
        {
            var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("No Authorization token found in the current context.");
            }

            if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = $"Bearer {token}";
            }

            var metadata = new Metadata
            {
                { "Authorization", token }
            };

            var callOptions = new CallOptions(metadata);
               var response = await _grpcClient.GetAllItemGroupsAsync(new GetAllItemGroupsRequest(), // Matches your proto
                new CallOptions(metadata));

            return response.Items
                .Select(x => new Contracts.Dtos.Inventory.ItemGroupDto
                {
                    Id = x.Id,
                    UnitId = x.UnitId,
                    ItemGroupCode = x.ItemGroupCode,
                    ItemGroupName = x.ItemGroupName,
                    IsActive = x.IsActive
                    
                })
                .ToList();
        }
    }
}