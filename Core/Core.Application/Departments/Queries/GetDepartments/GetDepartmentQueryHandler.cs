using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Data;
using Core.Application.Common.Interfaces.IDepartment;
using Core.Application.Common.HttpResponse;


namespace Core.Application.Departments.Queries.GetDepartments
{
    public class GetDepartmentQueryHandler :IRequestHandler<GetDepartmentQuery,ApiResponseDTO<List<DepartmentDto>>>
    {
        private readonly IDepartmentQueryRepository _departmentRepository;
        private readonly IMapper _mapper; 

     public GetDepartmentQueryHandler(IDepartmentQueryRepository divisionRepository,IMapper mapper)
        {
            _mapper =mapper;
            _departmentRepository = divisionRepository;                
        }

        public async Task<ApiResponseDTO<List<DepartmentDto>>> Handle(GetDepartmentQuery request ,CancellationToken cancellationToken )
        {
           
              var result = await _departmentRepository.GetAllDepartmentAsync();
              var department = _mapper.Map<List<DepartmentDto>>(result);
              
                return new ApiResponseDTO<List<DepartmentDto>> { IsSuccess = true, Message = "Success", Data = department };  
           
        }

      
    }
}