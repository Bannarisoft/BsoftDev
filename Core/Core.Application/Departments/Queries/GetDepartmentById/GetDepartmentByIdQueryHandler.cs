using Core.Application.Departments.Queries.GetDepartments;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace Core.Application.Departments.Queries.GetDepartmentById
{
    public class GetDepartmentByIdQueryHandler :IRequestHandler<GetDepartmentByIdQuery,DepartmentDto>
    {
          private readonly IDbConnection _dbConnection;
        private readonly IMapper _mapper;
          public GetDepartmentByIdQueryHandler(IDbConnection dbConnection)
          {
            _dbConnection = dbConnection;
           
          }
           public async Task<DepartmentDto?> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
           {
        var query = "SELECT CompanyId,ShortName,DeptName,IsActive  FROM AppData.Country WHERE Id = @Id";
        var department = await _dbConnection.QuerySingleOrDefaultAsync<DepartmentDto>(query, new { Id = request.DepartmentId });
        // Return null if the country is not found
        if (department == null)
          {
            return null;
          }

        // Map the country entity to a DTO
        return new DepartmentDto
         {
            Id = department.Id,
            CompanyId = department.CompanyId,
            ShortName = department.ShortName,
            DeptName = department.DeptName,
            IsActive = department.IsActive
          };
        }

          // public async Task<DepartmentVm> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
          // {
          //   var department =await _departmentRepository.GetByIdAsync(request.DepartmentId);
          //   return _mapper.Map<DepartmentVm>(department);
          // }

          


    }
}