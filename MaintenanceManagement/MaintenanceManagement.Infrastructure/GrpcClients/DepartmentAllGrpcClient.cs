using Contracts.Dtos.Users;
using Contracts.Interfaces.External.IUser;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServices.UserManagement;
using Microsoft.AspNetCore.Http;

namespace MaintenanceManagement.Infrastructure.GrpcClients
{
    public class DepartmentAllGrpcClient : IDepartmentAllGrpcClient
    {
        private readonly DepartmentAllService.DepartmentAllServiceClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DepartmentAllGrpcClient(DepartmentAllService.DepartmentAllServiceClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Contracts.Dtos.Users.DepartmentAllDto>> GetDepartmentAllAsync()
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

            var response = await _client.GetDepartmentAllAsync(new Empty(), callOptions);

            return response.Departments.Select(proto => new Contracts.Dtos.Users.DepartmentAllDto
            {
                DepartmentId = proto.DepartmentId,
                DepartmentName = proto.DepartmentName,
                ShortName = proto.ShortName,
                Departmentgroupid = proto.DepartmentGroupId
            }).ToList();
        }

    }
}