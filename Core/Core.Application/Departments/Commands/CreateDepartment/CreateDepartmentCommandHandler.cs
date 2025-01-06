using Core.Application.Departments.Queries.GetDepartments;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IDepartment;

namespace Core.Application.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommandHandler :IRequestHandler<CreateDepartmentCommand, DepartmentDto >
    {
        private readonly IDepartmentCommandRepository _departmentRepository;
        private readonly IMapper _mapper;
           
    
         public CreateDepartmentCommandHandler(IDepartmentCommandRepository departmentRepository,IMapper mapper)
        {
             _departmentRepository=departmentRepository;
            _mapper=mapper;
        }

        // public async Task<DepartmentVm>Handle(CreateDepartmentCommand request,CancellationToken cancellationToken)
        // {
        //     var departmentEntity=new Department
        //     {
        //     ShortName=request.ShortName,
        //     DeptName =request.DeptName,
        //     CompanyId=request.CompanyId,            
        //     IsActive=request.IsActive

           
        //     };
        //      var result=await _departmentRepository.CreateAsync(departmentEntity);
        //     return _mapper.Map<DepartmentVm>(result);
        //  }

       public async Task<DepartmentDto> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
              var departmentEntity = _mapper.Map<Department>(request);
          //  departmentEntity.Id = ID; // Assign the new GUID to the user entity

            // Save the user to the repository
            var createdDepartment = await _departmentRepository.CreateAsync(departmentEntity);
            
            if (createdDepartment == null)
            {
                throw new InvalidOperationException("Failed to create user");
            }

            // Map the created User entity to UserDto
            return _mapper.Map<DepartmentDto>(createdDepartment);
        }
    }  
}