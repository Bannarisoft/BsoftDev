using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Dtos.Inventory;
using Contracts.Interfaces.External.IInvetoryManagement;
using DnsClient.Internal;
using Grpc.Core;
using Inventory.Grpc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WarehouseManagement.Infrastructure.GrpcClients
{
    public class InventoryMiscMasterGrpcClient : IMiscMasterGrpcClient
    {

        private readonly MiscMasterService.MiscMasterServiceClient _grpcClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ILogger<InventoryMiscMasterGrpcClient> _logger;


        // private readonly ILogger _logger;

        public InventoryMiscMasterGrpcClient(MiscMasterService.MiscMasterServiceClient grpcClient, IHttpContextAccessor httpContextAccessor, ILogger<InventoryMiscMasterGrpcClient> logger)
        {
            _grpcClient = grpcClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

        }

               private Metadata BuildAuthHeadersOrNull()
        {
            var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(token)) return null;
            if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                token = $"Bearer {token}";
            return new Metadata { { "Authorization", token } };
        }

                public async Task<List<Contracts.Dtos.Inventory.MiscMasterDto>> GetMiscMasterByIdAsync(string miscType)
            {
                if (string.IsNullOrWhiteSpace(miscType))
                    throw new ArgumentException("misctype is required.", nameof(miscType));

                var headers = BuildAuthHeadersOrNull();

                var resp = await _grpcClient.GetMiscMasterByIdAsync(
                    new GetMiscMasterByIdRequest { Misctype = miscType.Trim() }, // NO ToLower()
                    headers: headers);

                return resp.Items.Select(x => new Contracts.Dtos.Inventory.MiscMasterDto {
                    Id = x.Id, Code = x.Code, Description = x.Description, MiscTypeId = x.MiscTypeId
                }).ToList();
            }

        public async Task<(int? WarehouseTypeId, int? StorageTypeId, int? AreaTypeId, int? OperationTypeId , int? FloorTypeId , int? AisleTypeId, int? RackLevelTypeId )>
            GetMiscTypeIdsAsync()
        {
            var headers = BuildAuthHeadersOrNull();

            var resp = await _grpcClient.GetMiscTypeIdsAsync(
                new GetMiscTypeIdsRequest(),
                headers: headers
             );

            return (resp.WarehouseTypeId, resp.StorageTypeId, resp.AreaTypeId, resp.OperationTypeId ,resp.FloorTypeId,resp.WarehouseAisleTypeId,resp.WarehouseRackLevelTypeId);
        }
        // public async Task<(int WarehouseTypeId, int StorageTypeId, int AreaTypeId, int OperationTypeId)> GetMiscTypeIdsAsync()
        //     {
        //         // forward bearer token if present
        //         var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();
        //         if (!string.IsNullOrWhiteSpace(token) && !token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        //             token = $"Bearer {token}";
        //         var headers = string.IsNullOrWhiteSpace(token) ? null : new Metadata { { "Authorization", token } };

        //         // gRPC call
        //         var resp = await _grpcClient.GetMiscMasterByIdAsync(
        //             new GetMiscMasterByIdRequest(),
        //             headers: headers);

        //         return (resp.WarehouseTypeId, resp.StorageTypeId, resp.AreaTypeId, resp.OperationTypeId);
        //     }
        //  public async Task<List<Contracts.Dtos.Inventory.MiscMasterDto>> GetMiscMasterByIdAsync(string miscType)
        // {
        //     if (string.IsNullOrWhiteSpace(miscType))
        //         throw new ArgumentException("misctype is required.", nameof(miscType));

        //     // normalize to match server mapping
        //     var key = miscType.Trim().ToLowerInvariant();

        //     // forward bearer token if present
        //     var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();
        //     if (!string.IsNullOrWhiteSpace(token) && !token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        //         token = $"Bearer {token}";
        //     var headers = string.IsNullOrWhiteSpace(token) ? null : new Metadata { { "Authorization", token } };

        //     // make the call
        //     var call = _grpcClient.GetMiscMasterByIdAsync(
        //         new GetMiscMasterByIdRequest { Misctype = key },
        //         headers: headers);

        //     // (optional) fail fast on auth/proxy before waiting for body
        //     await call.ResponseHeadersAsync.ConfigureAwait(false);

        //     var response = await call.ResponseAsync.ConfigureAwait(false);

        //     return response.Items
        //         .Select(x => new Contracts.Dtos.Inventory.MiscMasterDto
        //         {
        //             Id = x.Id,
        //             Code = x.Code,
        //             Description = x.Description,
        //             MiscTypeId = x.MiscTypeId
        //         })
        //         .ToList();
        // }
        //  public async Task<List<MiscMasterDto>> GetMiscMasterByIdAsync(string miscType)
        // {
        //     var request = new GetMiscMasterByIdRequest { Misctype = miscType };
        //     var response = await _grpcClient.GetMiscMasterByIdAsync(request);

        //     return response.Items
        //         .Select(x => new MiscMasterDto
        //         {
        //             Id = x.Id,
        //             Code = x.Code,
        //             Description = x.Description,
        //             MiscTypeId = x.MiscTypeId
        //         })
        //         .ToList();
        // }
        // private string GetBearerOrThrow()
        //     {
        //         var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();
        //         if (string.IsNullOrWhiteSpace(token))
        //             throw new InvalidOperationException("No Authorization token found in the current context.");
        //         if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        //             token = $"Bearer {token}";
        //         return token;
        //     }

        //  public async Task<List<Contracts.Dtos.Inventory.MiscMasterDto>> GetMiscMasterByIdAsync(string miscType)
        //         {
        //             if (string.IsNullOrWhiteSpace(miscType))
        //                 throw new ArgumentException("misctype is required.", nameof(miscType));

        //             var key = miscType.Trim().ToLowerInvariant();
        //             var headers = new Metadata { { "Authorization", GetBearerOrThrow() } };
        //             var deadline = DateTime.UtcNow.AddSeconds(30);

        //             var resp = await _grpcClient.GetMiscMasterByIdAsync(
        //                 new GetMiscMasterByIdRequest { Misctype = key },
        //                 headers: headers,
        //                 deadline: deadline
        //                 );

        //             return resp.Items.Select(x => new Contracts.Dtos.Inventory.MiscMasterDto
        //             {
        //                 Id = x.Id,
        //                 Code = x.Code,
        //                 Description = x.Description,
        //                 MiscTypeId = x.MiscTypeId
        //             }).ToList();
        //         }

        // public async Task<List< Contracts.Dtos.Inventory.MiscMasterDto>> GetMiscMasterByIdAsync(  string miscType, CancellationToken ct)
        //             {
        //                 if (string.IsNullOrWhiteSpace(miscType))
        //                     throw new ArgumentException("misctype is required.", nameof(miscType));

        //                 // Prefer an interceptor/CallCredentials; headers are okay if you must.
        //                 var headers = new Metadata { { "Authorization", GetBearerOrThrow() } };

        //                 var deadline = DateTime.UtcNow.AddSeconds(_options.GrpcTimeoutSeconds); // from config

        //                 try
        //                 {
        //                     var resp = await _grpcClient.GetMiscMasterByIdAsync(
        //                         new GetMiscMasterByIdRequest { Misctype = miscType.Trim().ToLowerInvariant() },
        //                         headers: headers,
        //                         deadline: deadline,
        //                         // Consider leaving this false in prod; true only if you expect cold-start gaps.
        //                         waitForReady: false,
        //                         cancellationToken: ct);

        //                     return resp.Items.Select(x => new MiscMasterDto
        //                     {
        //                         Id = x.Id, Code = x.Code, Description = x.Description, MiscTypeId = x.MiscTypeId
        //                     }).ToList();
        //                 }
        //                 catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        //                 {
        //                     _logger.LogWarning(ex, "gRPC unavailable. Will rely on caller-level retry/backoff.");
        //                     throw; // let a higher-level retry policy (Polly/service config) handle it
        //                 }
        //                 catch (RpcException ex)
        //                 {
        //                     _logger.LogError(ex, "gRPC failed. Status:{Status} Detail:{Detail}", ex.StatusCode, ex.Status.Detail);
        //                     // map to domain exceptions if your API layer expects them
        //                     throw;
        //                 }
        //             }


        // public async Task<List<Contracts.Dtos.Inventory.MiscMasterDto>> GetMiscMasterByIdAsync(string miscType)
        // {
        //     // 1) Token
        //     var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();
        //     if (string.IsNullOrWhiteSpace(token))
        //         throw new InvalidOperationException("No Authorization token found in the current context.");
        //     if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        //         token = $"Bearer {token}";

        //     var headers = new Metadata { { "Authorization", token } };

        //     // 2) Validate + normalize miscType on the CLIENT before calling the server
        //     if (string.IsNullOrWhiteSpace(miscType))
        //         throw new ArgumentException("misctype is required.", nameof(miscType));

        //     var key = miscType.Trim().ToLowerInvariant();
        //     // (optional) constrain to known values to avoid server-side InvalidArgument
        //     if (key is not ("warehouse" or "warehousetype" or
        //                     "storage"   or "storagetype"   or
        //                     "area"      or "areatype"      or
        //                     "operation" or "operationtype"))
        //         throw new ArgumentException($"Unsupported misctype: {miscType}", nameof(miscType));

        //     try
        //     {
        //         var resp = await _grpcClient.GetMiscMasterByIdAsync(
        //             new GetMiscMasterByIdRequest { Misctype = key },
        //             headers: headers,
        //             deadline: DateTime.UtcNow.AddSeconds(10));

        //         return resp.Items.Select(x => new Contracts.Dtos.Inventory.MiscMasterDto
        //         {
        //             Id = x.Id,
        //             Code = x.Code,
        //             Description = x.Description,
        //             MiscTypeId = x.MiscTypeId
        //         }).ToList();
        //     }
        //     catch (RpcException ex)
        //     {
        //         // 🔎 DO NOT comment this out — it tells you exactly what's wrong
        //         _logger.LogError(ex, "GetMiscMasterById gRPC failed. Status:{Status} Detail:{Detail}",
        //             ex.StatusCode, ex.Status.Detail);

        //         // Optional: map to domain-specific exceptions so your API can translate to proper HTTP
        //         throw ex.StatusCode switch
        //         {
        //             StatusCode.InvalidArgument => new ArgumentException(ex.Status.Detail),
        //             StatusCode.Unauthenticated => new UnauthorizedAccessException(ex.Status.Detail),
        //             StatusCode.PermissionDenied => new UnauthorizedAccessException(ex.Status.Detail),
        //             StatusCode.NotFound => new KeyNotFoundException(ex.Status.Detail),
        //             _ => new Exception($"Inventory gRPC error ({ex.StatusCode}): {ex.Status.Detail}", ex)
        //         };
        //     }
        // }

        //  public async Task<List<Contracts.Dtos.Inventory.MiscMasterDto>> GetMiscMasterByIdAsync(string miscType)
        // {
        //     var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();
        //     if (string.IsNullOrWhiteSpace(token))
        //         throw new Exception("No Authorization token found in the current context.");

        //     if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        //         token = $"Bearer {token}";

        //     var metadata = new Metadata { { "Authorization", token } };

        //     try
        //     {
        //         // normalize misctype going out
        //         var normalized = miscType?.Trim();
        //         var resp = await _grpcClient.GetMiscMasterByIdAsync(
        //             new GetMiscMasterByIdRequest { Misctype = normalized },
        //             headers: metadata); // <- simpler overload

        //         return resp.Items.Select(x => new Contracts.Dtos.Inventory.MiscMasterDto
        //         {
        //             Id = x.Id, Code = x.Code, Description = x.Description, MiscTypeId = x.MiscTypeId
        //         }).ToList();
        //     }
        //     catch (RpcException ex)
        //     {
        //         // _logger.LogError(ex, "GetMiscMasterById failed. Status:{Status} Detail:{Detail}",
        //          //   ex.StatusCode, ex.Status.Detail);
        //         // Optionally map to your domain exception
        //         throw;
        //     }
        // }


        // public async Task<List<Contracts.Dtos.Inventory.MiscMasterDto>> GetMiscMasterByIdAsync(string miscType)
        // {
        //      var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();

        //     if (string.IsNullOrEmpty(token))
        //     {
        //         throw new Exception("No Authorization token found in the current context.");
        //     }

        //     if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        //     {
        //         token = $"Bearer {token}";
        //     }

        //     var metadata = new Metadata
        //     {
        //         { "Authorization", token }
        //     };

        //     var callOptions = new CallOptions(metadata);
        //     var response = await _grpcClient.GetMiscMasterByIdAsync(
        //         new GetMiscMasterByIdRequest { Misctype = miscType },callOptions);

        //     return response.Items
        //         .Select(x => new Contracts.Dtos.Inventory.MiscMasterDto
        //         {
        //             Id = x.Id,
        //             Code = x.Code,
        //             Description = x.Description,
        //             MiscTypeId = x.MiscTypeId
        //         })
        //         .ToList();
        // }
    }
}