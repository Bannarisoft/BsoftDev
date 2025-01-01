using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Core.Application.Departments.Queries.GetDepartmentAutoComplete;
using System.Data;
using Core.Application.Departments.Queries.GetDepartments;


namespace Core.Application.Departments.Queries.GetDepartmentAutoCompleteSearch
{
    public class GetDepartmentAutoCompleteSearchQueryHandler : IRequestHandler<GetDepartmentAutoCompleteSearchQuery, List<DepartmentDto>>
    {
      private readonly IDbConnection _dbConnection;

        public GetDepartmentAutoCompleteSearchQueryHandler(IDbConnection dbConnection)
        {
             _dbConnection = dbConnection;
        }

        public async Task<List<DepartmentDto>> Handle(GetDepartmentAutoCompleteSearchQuery request, CancellationToken cancellationToken)
        {
            var query = @"
            select Id,CompanyId,ShortName,DeptName,IsActive from  AppData.Department 
            WHERE DeptName LIKE @SearchPattern OR Id LIKE @SearchPattern and IsActive =1
            ORDER BY DeptName";
        // var query = @"
        //             SELECT Id, countryCode, countryName, IsActive
        //             FROM AppData.Country
        //             WHERE countryName LIKE @SearchPattern OR countryCode LIKE @SearchPattern
        //             ORDER BY countryName";
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
                }).ToList();

            
           
            
        }
        }
         
    }

