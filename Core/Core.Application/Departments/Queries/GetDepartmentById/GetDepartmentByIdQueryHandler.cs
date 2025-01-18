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
using System.Data;
using Core.Application.Common.Interfaces.IDepartment;
using Core.Application.Common.HttpResponse;

namespace Core.Application.Departments.Queries.GetDepartmentById
{
    public class GetDepartmentByIdQueryHandler :IRequestHandler<GetDepartmentByIdQuery,ApiResponseDTO<DepartmentDto>>
    {
          private readonly IDepartmentQueryRepository _departmentRepository;        
        private readonly IMapper _mapper;

        public GetDepartmentByIdQueryHandler(IDepartmentQueryRepository departmentRepository,IMapper mapper)
         {
            _departmentRepository = departmentRepository;
            _mapper =mapper;
        } 
      public async Task<ApiResponseDTO<DepartmentDto>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
              var user = await _departmentRepository.GetByIdAsync(request.DepartmentId);
              var department = _mapper.Map<DepartmentDto>(user);
            return new ApiResponseDTO<DepartmentDto> { IsSuccess = true, Message = "Success", Data = department };

        }
 


    }
}