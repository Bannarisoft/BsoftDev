using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using Core.Application.Departments.Queries.GetDepartments;
using Core.Application.Common.Interfaces.IDepartment;


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
           /*  var query = @"
            select Id,CompanyId,ShortName,DeptName,IsActive from  AppData.Department 
            WHERE DeptName LIKE @SearchPattern OR Id LIKE @SearchPattern and IsActive =1
            ORDER BY DeptName";
        
            // Execute the query and map the result to a list of CountryDto
                var department = await _dbConnection.QueryAsync<DepartmentDto>(
                    query, 
                    new { SearchPattern = $"%{request.SearchPattern}%" }  // Use the search pattern with wildcards
                );
                if (department == null || !department.Any())
                {
                    return new List<DepartmentDto>(); // Return empty list if no matches are found
                }

                // Map the results to DTOs
                return department.Select(Department => new DepartmentDto
                {
                    Id = Department.Id,
                    ShortName = Department.ShortName,
                    DeptName = Department.DeptName,
                    IsActive = Department.IsActive
                }).ToList(); */
                  var result = await _departmentRepository.GetAllDepartmentAutoCompleteSearchAsync(request.SearchPattern);
                //return _mapper.Map<List<DivisionDTO>>(result);
                return _mapper.Map<List<DepartmentDto>>(result);     

            
           
            
        }
        }
         
    }

