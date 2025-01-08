using Dapper;
using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Data;


namespace Core.Application.Departments.Queries.GetDepartments
{
    public class GetDepartmentQueryHandler :IRequestHandler<GetDepartmentQuery,List<DepartmentDto>>
    {
       private readonly IDbConnection _dbConnection;
    //   private readonly IDepartmentRepository _departmentRepository;
     private readonly IMapper _mapper;


     public GetDepartmentQueryHandler(IDbConnection dbConnection)
        {
          _dbConnection = dbConnection;
            
        }

        public async Task<List<DepartmentDto>> Handle(GetDepartmentQuery request ,CancellationToken cancellationToken )
        {
            const string query = @"SELECT  * FROM AppData.Department";
            var department = await _dbConnection.QueryAsync<DepartmentDto>(query);
           return department.AsList();
           
        }      
    }
}