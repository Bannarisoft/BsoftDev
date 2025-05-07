// using Grpc.Core;
// using GrpcServices.Maintenance;
using Microsoft.AspNetCore.Http;
// using Contracts.Interfaces.External.IUser;

namespace MaintenanceManagement.Infrastructure.GrpcClients
{
    // public class DepartmentGrpcClient : IDepartmentGrpcClient
    // {
    //     private readonly DepartmentService.DepartmentServiceClient _client;
    //     private readonly IHttpContextAccessor _httpContextAccessor;

    //     public DepartmentGrpcClient(DepartmentService.DepartmentServiceClient client, IHttpContextAccessor httpContextAccessor)
    //     {
    //         _client = client;
    //         _httpContextAccessor = httpContextAccessor;
    //     }

    //     public async Task<List<Contracts.Dtos.Maintenance.DepartmentDto>> GetAllDepartmentsAsync()
    //     {
    //         // ✅ Get token from current HTTP Context
    //         var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();

    //         if (string.IsNullOrEmpty(token))
    //         {
    //             throw new Exception("No Authorization token found in the current context.");
    //         }
    //         // ✅ Ensure it has "Bearer " prefix
    //         if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
    //         {
    //             token = $"Bearer {token}";
    //         }

    //         var metadata = new Metadata
    //         {
    //             { "Authorization", token }
    //         };

    //         // ✅ Attach Authorization header
    //         var callOptions = new CallOptions(metadata);

    //         var response = await _client.GetAllDepartmentsAsync(new Empty(), callOptions);

    //         var departments = response.Departments
    //             .Select(proto => new Contracts.Dtos.Maintenance.DepartmentDto
    //             {
    //                 DepartmentId = proto.DepartmentId,
    //                 DepartmentName = proto.DepartmentName,
    //                 ShortName = proto.ShortName
    //             }).ToList();

    //         return departments;
    //     }
    // }
}
