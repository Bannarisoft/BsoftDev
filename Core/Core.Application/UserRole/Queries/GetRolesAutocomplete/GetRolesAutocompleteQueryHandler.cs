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
using Core.Application.UserRole.Queries.GetRole;

namespace Core.Application.UserRole.Queries.GetRolesAutocomplete
{
    public class GetRolesAutocompleteQueryHandler : IRequestHandler<GetRolesAutocompleteQuery, List<UserRoleDto>>
    {
         private readonly IDbConnection _dbConnection;

        public GetRolesAutocompleteQueryHandler(IDbConnection dbConnection)
        {
          _dbConnection = dbConnection;
        }

        public async Task<List<UserRoleDto>> Handle(GetRolesAutocompleteQuery request, CancellationToken cancellationToken)
        {

              var query = @"
            select Id,RoleName,Description,CompanyId,IsActive from  AppSecurity.UserRole 
            WHERE RoleName LIKE @SearchPattern OR Id LIKE @SearchPattern and IsActive =1
            ORDER BY RoleName";
       // Execute the query and map the result to a list of CountryDto
        var userrole = await _dbConnection.QueryAsync<UserRoleDto>(
            query, 
            new { SearchPattern = $"%{request.SearchPattern}%" }  // Use the search pattern with wildcards
        );
        if (userrole == null || !userrole.Any())
        {
            return new List<UserRoleDto>(); // Return empty list if no matches are found
        }

        // Map the results to DTOs
        return userrole.Select(UserRole => new UserRoleDto
        {
            UserRoleId = UserRole.UserRoleId,           
            RoleName = UserRole.RoleName,
            Description =UserRole.Description,
            CompanyId=UserRole.CompanyId,
            IsActive=UserRole.IsActive
            
        }).ToList();
           
        }
        
    }
}