using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using Core.Application.Common.Interfaces.IDepartment;
using Core.Application.Departments.Queries.GetDepartments;

namespace Core.Application.Departments.Queries.GetDepartmentAutoCompleteSearch
{
    public class GetDepartmentAutoCompleteSearchQueryHandler : IRequestHandler<GetDepartmentAutoCompleteSearchQuery, List<DepartmentDto>>
    {
        private readonly IDepartmentQueryRepository _departmentRepository;
        private readonly IMapper _mapper;
        public GetDepartmentAutoCompleteSearchQueryHandler(IDepartmentQueryRepository divisionRepository,IMapper mapper)
        {
             _mapper =mapper;
            _departmentRepository = divisionRepository;            
        }

        public async Task<List<DepartmentDto>> Handle(GetDepartmentAutoCompleteSearchQuery request, CancellationToken cancellationToken)
        {
               var result = await _departmentRepository.GetAllDepartmentAutoCompleteSearchAsync(request.SearchPattern);               
                return _mapper.Map<List<DepartmentDto>>(result);     

            
           
            
        }
        }
         
    }

