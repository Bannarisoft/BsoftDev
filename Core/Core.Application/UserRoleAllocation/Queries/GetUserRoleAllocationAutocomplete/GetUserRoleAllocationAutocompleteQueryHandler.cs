using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using Dapper;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Application.UserRoleAllocation.Queries.GetUserRoleAllocation;

namespace Core.Application.UserRoleAllocation.Queries.GetUserRoleAllocationAutocomplete
{
    public class GetUserRoleAllocationAutocompleteQueryHandler : IRequestHandler<GetUserRoleAllocationAutocompleteQuery, List<CreateUserRoleAllocationDto>>
    {
         private readonly IDbConnection _dbConnection;
        private readonly IMapper _mapper;

        public GetUserRoleAllocationAutocompleteQueryHandler(IDbConnection dbConnection, IMapper mapper)
        {
          _dbConnection = dbConnection;
          _mapper = mapper;
        }

        public async Task<List<CreateUserRoleAllocationDto>> Handle(GetUserRoleAllocationAutocompleteQuery request, CancellationToken cancellationToken)
        {
            const string query = @"
                SELECT 
                    Id AS UserRoleId,
                    RoleName,
                    Description,
                    CompanyId,
                    IsActive
                FROM 
                    AppSecurity.UserRole
                WHERE 
                    (RoleName LIKE @SearchPattern OR CAST(Id AS VARCHAR) LIKE @SearchPattern)
                    AND IsActive = 1
                ORDER BY 
                    RoleName";

            // Execute the query with the search pattern
            var userRoles = await _dbConnection.QueryAsync<UserRoleAllocationResponseDto>(
                query,
                new { SearchPattern = $"%{request.SearchPattern}%" }
            );

            // Return an empty list if no matches are found
            if (userRoles == null || !userRoles.Any())
            {
                return new List<CreateUserRoleAllocationDto>();
            }

            // Map the query results to CreateUserRoleAllocationDto
            var result = _mapper.Map<List<CreateUserRoleAllocationDto>>(userRoles);
            return result;
           
        }
        
    }
}