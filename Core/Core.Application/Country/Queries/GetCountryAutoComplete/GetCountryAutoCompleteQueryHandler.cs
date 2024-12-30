using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interface;
using Core.Application.Country.Queries.GetCountries;
using Core.Application.Country.Queries.GetCountryAutoComplete;
using AutoMapper;
using Dapper;
using MediatR;

namespace Core.Application.Country.Queries.GetCountryAutoComplete
{
    public class GetCountryAutoCompleteQueryHandler : IRequestHandler<GetcountryAutoCompleteQuery, List<CountryDto>>
    
{
     private readonly IDbConnection _dbConnection;

    public GetCountryAutoCompleteQueryHandler(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<List<CountryDto>> Handle(GetcountryAutoCompleteQuery request, CancellationToken cancellationToken)
    {
          var query = @"
            SELECT Id, countryCode, countryName, IsActive
            FROM AppData.Country
            WHERE countryName LIKE @SearchPattern OR countryCode LIKE @SearchPattern
            ORDER BY countryName";
       // Execute the query and map the result to a list of CountryDto
        var countries = await _dbConnection.QueryAsync<CountryDto>(
            query, 
            new { SearchPattern = $"%{request.SearchPattern}%" }  // Use the search pattern with wildcards
        );
        if (countries == null || !countries.Any())
        {
            return new List<CountryDto>(); // Return empty list if no matches are found
        }

        // Map the results to DTOs
        return countries.Select(country => new CountryDto
        {
            Id = country.Id,
            CountryCode = country.CountryCode,
            CountryName = country.CountryName,
            IsActive = country.IsActive
        }).ToList();
    }
}
  
}