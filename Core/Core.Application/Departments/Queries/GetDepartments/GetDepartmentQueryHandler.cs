using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Data;
using Core.Application.Common.Interfaces.IDepartment;


namespace Core.Application.Departments.Queries.GetDepartments
{
    public class GetDepartmentQueryHandler :IRequestHandler<GetDepartmentQuery,List<DepartmentDto>>
    {
        private readonly IDepartmentQueryRepository _departmentRepository;
        private readonly IMapper _mapper; 

     public GetDepartmentQueryHandler(IDepartmentQueryRepository divisionRepository,IMapper mapper)
        {
            _mapper =mapper;
            _departmentRepository = divisionRepository;                
        }

        public async Task<List<DepartmentDto>> Handle(GetDepartmentQuery request ,CancellationToken cancellationToken )
        {
           /*  const string query = @"SELECT  * FROM AppData.Department";
            var department = await _dbConnection.QueryAsync<DepartmentDto>(query);
           return department.AsList(); */
              var result = await _departmentRepository.GetAllDepartmentAsync();
                //return _mapper.Map<List<DivisionDTO>>(result);
                return _mapper.Map<List<DepartmentDto>>(result);  
           
        }

      
    }
}