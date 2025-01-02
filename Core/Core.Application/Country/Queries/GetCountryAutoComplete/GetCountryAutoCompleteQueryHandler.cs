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
    public class GetCountryAutoCompleteQueryHandler : IRequestHandler<GetCountryAutoCompleteQuery, List<CountryDto>>
    
{
     private readonly IDbConnection _dbConnection;

    public GetCountryAutoCompleteQueryHandler(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<List<CountryDto>> Handle(GetCountryAutoCompleteQuery request, CancellationToken cancellationToken)
    {
          var query = @"
            SELECT Id, countryCode, countryName, IsActive,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP
            FROM AppData.Country with (nolock)
            WHERE countryName LIKE @SearchPattern OR countryCode LIKE @SearchPattern and IsActive = 1
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

       return countries.AsList();  
    }
}
  
}