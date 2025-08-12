using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Interfaces.External.IInvetoryManagement;
using Grpc.Core;
using Inventory.Grpc;
using Microsoft.AspNetCore.Http;

namespace WarehouseManagement.Infrastructure.GrpcClients
{
    public class InventoryUOMGrpcClient : IUOMGrpcClient
    {
        private readonly InventoryUOMService.InventoryUOMServiceClient _inventoryUOMServiceClient;

        private readonly IHttpContextAccessor _httpContextAccessor;


        public InventoryUOMGrpcClient(InventoryUOMService.InventoryUOMServiceClient inventoryUOMServiceClient, IHttpContextAccessor httpContextAccessor)
        {
            _inventoryUOMServiceClient = inventoryUOMServiceClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Contracts.Dtos.Inventory.UOMDto>> GetUOMAsync()
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
            var response = await _inventoryUOMServiceClient.GetAllUOMsAsync(new GetAllUOMsRequest(), // Matches your proto
            new CallOptions(metadata));
                return response.Items
                .Select(x => new Contracts.Dtos.Inventory.UOMDto
                {
                    
                    Id = x.Id,
                    Code = x.Code,
                    UOMName = x.UomName,
                    UOMTypeId = x.UomTypeId,                  
                    IsActive = x.IsActive
                })
                .ToList();

        }
    }
}