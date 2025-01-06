using Dapper;
using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using System.Data;
using Core.Application.Common.Interfaces.IUserRole;


namespace Core.Application.UserRole.Queries.GetRole
{
    public class GetRoleQueryHandler :IRequestHandler<GetRoleQuery,List<UserRoleDto>>
    {
       private readonly IDbConnection _dbConnection;
     private readonly IUserRoleQueryRepository _IuserRoleRepository;
     private readonly IMapper _Imapper;
   


       public GetRoleQueryHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<UserRoleDto>> Handle(GetRoleQuery request ,CancellationToken cancellationToken )
        {
            const string query = @"SELECT  * FROM AppSecurity.UserRole";
             var department = await _dbConnection.QueryAsync<UserRoleDto>(query);
           return department.AsList();
        }



  }
}