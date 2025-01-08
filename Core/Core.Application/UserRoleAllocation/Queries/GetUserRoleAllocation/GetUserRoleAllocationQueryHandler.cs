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
using Core.Application.Common.Interfaces.IUserRoleAllocation;


namespace Core.Application.UserRoleAllocation.Queries.GetUserRoleAllocation
{
    public class GetUserRoleAllocationQueryHandler :IRequestHandler<GetUserRoleAllocationQuery,List<CreateUserRoleAllocationDto>>
    {
       private readonly IDbConnection _dbConnection;
       private readonly IMapper _mapper;
   


       public GetUserRoleAllocationQueryHandler(IDbConnection dbConnection, IMapper mapper)
        {
            _dbConnection = dbConnection;
            _mapper = mapper;

        }

        public async Task<List<CreateUserRoleAllocationDto>> Handle(GetUserRoleAllocationQuery request ,CancellationToken cancellationToken )
        {
            const string query = @"
                SELECT 
                    ura.Id, 
                    ura.UserId, 
                    ura.UserRoleId, 
                    ur.RoleName 
                FROM 
                    AppSecurity.UserRoleAllocation ura
                INNER JOIN 
                    AppSecurity.UserRole ur 
                ON 
                    ura.UserRoleId = ur.Id";

            // Execute the query and map results to the DTO
            var result = await _dbConnection.QueryAsync<UserRoleAllocationResponseDto>(query);

            // Map the result to the required DTO
            var mappedResult = _mapper.Map<List<CreateUserRoleAllocationDto>>(result);

            return mappedResult;
        }



  }
}