using BSOFT.Application.Departments.Queries.GetDepartments;
using BSOFT.Domain.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Departments.Queries.GetDepartmentById
{
    public class GetDepartmentByIdQueryHandler :IRequestHandler<GetDepartmentByIdQuery,DepartmentVm>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
          public GetDepartmentByIdQueryHandler(IDepartmentRepository departmentRepository, IMapper mapper)
          {
            _departmentRepository=departmentRepository;
            _mapper=mapper;
          }

          public async Task<DepartmentVm> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
          {
            var department =await _departmentRepository.GetByIdAsync(request.DepartmentId);
            return _mapper.Map<DepartmentVm>(department);
          }


    }
}