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
    public class InventoryMiscMasterGrpcClient : IMiscMasterGrpcClient
    {

        private readonly MiscMasterService.MiscMasterServiceClient _grpcClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InventoryMiscMasterGrpcClient(MiscMasterService.MiscMasterServiceClient grpcClient, IHttpContextAccessor httpContextAccessor)
        {
            _grpcClient = grpcClient;
            _httpContextAccessor = httpContextAccessor;
        }
       
        
        public async Task<List<Contracts.Dtos.Inventory.MiscMasterDto>> GetMiscMasterByIdAsync(string miscType)
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
            var response = await _grpcClient.GetMiscMasterByIdAsync(
                new GetMiscMasterByIdRequest { Misctype = miscType },callOptions);

            return response.Items
                .Select(x => new Contracts.Dtos.Inventory.MiscMasterDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description,
                    MiscTypeId = x.MiscTypeId
                })
                .ToList();
        }
    }
}