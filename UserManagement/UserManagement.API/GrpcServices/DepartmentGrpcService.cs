/* using Grpc.Core;
using Core.Application.Common.Interfaces.IDepartment;
using Google.Protobuf.WellKnownTypes;
using Contracts.Dtos.Maintenance;
using GrpcServices.Maintenance;
using Microsoft.AspNetCore.Authorization; */

//[Authorize]
public class DepartmentGrpcService //: DepartmentService.DepartmentServiceBase
{
   /*  private readonly IDepartmentQueryRepository _departmentRepository;

    public DepartmentGrpcService(IDepartmentQueryRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public override async Task<DepartmentListResponse> GetAllDepartments(Empty request, ServerCallContext context)
    {
        var (departments, _) = await _departmentRepository.GetAllDepartmentAsync(1, 100, null);

        var response = new DepartmentListResponse();
        response.Departments.AddRange(departments.Select(d => new GrpcServices.Maintenance.DepartmentDto
        {
            DepartmentId = d.Id,
            DepartmentName = d.DeptName,
            ShortName = d.ShortName
        }));

        return response;
    } */
}
