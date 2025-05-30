
using Core.Application.Common.Interfaces.IDepartment;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServices.UserManagement;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class DepartmentAllGrpcService : DepartmentAllService.DepartmentAllServiceBase
{
    private readonly IDepartmentQueryRepository _departmentRepository;

    public DepartmentAllGrpcService(IDepartmentQueryRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }
    public override async Task<DepartmentAllListResponse> GetDepartmentAll(Empty request, ServerCallContext context)
    {
        var departments = await _departmentRepository.GetDepartment_SuperAdmin("");

        var response = new DepartmentAllListResponse();
        response.Departments.AddRange(departments.Select(d => new DepartmentAllDto
        {
            DepartmentId = d.Id,
            DepartmentName = d.DeptName,
            ShortName = d.ShortName,
            DepartmentGroupId = d.DepartmentGroupId

        }));

        return response;
    }
}
