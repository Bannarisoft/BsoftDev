using Core.Application.Departments.Queries.GetDepartments;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommandHandler :IRequestHandler<CreateDepartmentCommand, DepartmentVm >
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
           
    
         public CreateDepartmentCommandHandler(IDepartmentRepository departmentRepository,IMapper mapper)
        {
             _departmentRepository=departmentRepository;
            _mapper=mapper;
        }

        public async Task<DepartmentVm>Handle(CreateDepartmentCommand request,CancellationToken cancellationToken)
        {
            var departmentEntity=new Department
            {
            ShortName=request.ShortName,
            DeptName =request.DeptName,
            CompanyId=request.CompanyId,            
            IsActive=request.IsActive

           
            };
             var result=await _departmentRepository.CreateAsync(departmentEntity);
            return _mapper.Map<DepartmentVm>(result);
         }
    }  
}