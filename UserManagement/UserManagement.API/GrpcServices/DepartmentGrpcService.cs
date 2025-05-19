
using Core.Application.Common.Interfaces.IDepartment;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServices.UserManagement;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class DepartmentGrpcService : DepartmentService.DepartmentServiceBase
 {
   private readonly IDepartmentQueryRepository _departmentRepository;

    public DepartmentGrpcService(IDepartmentQueryRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public override async Task<DepartmentListResponse> GetAllDepartment(Empty request, ServerCallContext context)
    {
        var (departments, _) = await _departmentRepository.GetAllDepartmentAsync(1, 100, null);

        var response = new DepartmentListResponse();
        response.Departments.AddRange(departments.Select(d => new GrpcServices.UserManagement.DepartmentDto
        {
            DepartmentId = d.Id,
            DepartmentName = d.DeptName,
            ShortName = d.ShortName
        }));

        return response;
    } 
 }
