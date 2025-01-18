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
using Core.Application.Common.HttpResponse;

namespace Core.Application.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommandHandler :IRequestHandler<CreateDepartmentCommand, ApiResponseDTO<DepartmentDto>>
    {
        private readonly IDepartmentCommandRepository _departmentRepository;
        private readonly IMapper _mapper;
           
    
         public CreateDepartmentCommandHandler(IDepartmentCommandRepository departmentRepository,IMapper mapper)
        {
             _departmentRepository=departmentRepository;
            _mapper=mapper;
        }


       public async Task<ApiResponseDTO<DepartmentDto>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
              var departmentEntity = _mapper.Map<Department>(request);
            var createdDepartment = await _departmentRepository.CreateAsync(departmentEntity);
            
            if (createdDepartment == null)
            {
                return new ApiResponseDTO<DepartmentDto> { IsSuccess = false, Message = "Department not created" };
            }
           var departmentDto = _mapper.Map<DepartmentDto>(createdDepartment);
            
            return new ApiResponseDTO<DepartmentDto> { IsSuccess = true, Message = "Department created successfully", Data = departmentDto };
        }
    }  
}