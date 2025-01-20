using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using Core.Application.Departments.Queries.GetDepartments;
using Core.Application.Common.Interfaces.IDepartment;
using Core.Application.Common.HttpResponse;


namespace Core.Application.Departments.Queries.GetDepartmentAutoCompleteSearch
{
    public class GetDepartmentAutoCompleteSearchQueryHandler : IRequestHandler<GetDepartmentAutoCompleteSearchQuery, ApiResponseDTO<List<DepartmentDto>>>
    {
        private readonly IDepartmentQueryRepository _departmentRepository;
        private readonly IMapper _mapper;
        public GetDepartmentAutoCompleteSearchQueryHandler(IDepartmentQueryRepository divisionRepository,IMapper mapper)
        {
             _mapper =mapper;
            _departmentRepository = divisionRepository;            
        }

        public async Task<ApiResponseDTO<List<DepartmentDto>>> Handle(GetDepartmentAutoCompleteSearchQuery request, CancellationToken cancellationToken)
        {
          
                  var result = await _departmentRepository.GetAllDepartmentAutoCompleteSearchAsync(request.SearchPattern);
                  var department =_mapper.Map<ApiResponseDTO<List<DepartmentDto>>>(result);   
                return new ApiResponseDTO<List<DepartmentDto>> { IsSuccess = true, Message = "Success", Data = department.Data };  

        }
    }
         
}

