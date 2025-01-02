using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using MediatR;
using Core.Application.City.Queries.GetCities;

namespace Core.Application.City.Queries.GetCityAutoComplete
{
    public class GetCityAutoCompleteQueryHandler : IRequestHandler<GetCityAutoCompleteQuery, List<CityDto>>
    
{
     private readonly IDbConnection _dbConnection;

    public GetCityAutoCompleteQueryHandler(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<List<CityDto>> Handle(GetCityAutoCompleteQuery request, CancellationToken cancellationToken)
    {
          var query = @"
            SELECT Id, CityCode, CityName, IsActive,StateId,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP
            FROM AppData.City with (nolock)
            WHERE CityName LIKE @SearchPattern OR CityCode LIKE @SearchPattern AND IsActive = 1
            ORDER BY CityName";
       // Execute the query and map the result to a list of CountryDto
        var cities = await _dbConnection.QueryAsync<CityDto>(
            query, 
            new { SearchPattern = $"%{request.SearchPattern}%" }  // Use the search pattern with wildcards
        );
        if (cities == null || !cities.Any())
        {
            return new List<CityDto>(); // Return empty list if no matches are found
        }
        return cities.AsList();    
    }
}
  
}