using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Departments.Queries.GetDepartments
{
    public class GetDepartmentQueryHandler :IRequestHandler<GetDepartmentQuery,List<DepartmentVm>>
    {

     private readonly IDepartmentRepository _departmentRepository;
     private readonly IMapper _mapper;


     public GetDepartmentQueryHandler(IDepartmentRepository departmentRepository,IMapper mapper)
        {
            _departmentRepository=departmentRepository;
            _mapper =mapper;
        }

        public async Task<List<DepartmentVm>> Handle(GetDepartmentQuery request ,CancellationToken cancellationToken )
        {
            Console.WriteLine("Hello Handler");
            var department=await _departmentRepository.GetAllDepartmentAsync();
            var deparmentList= _mapper.Map<List<DepartmentVm>>(department);
            return deparmentList;
        }
    }
}