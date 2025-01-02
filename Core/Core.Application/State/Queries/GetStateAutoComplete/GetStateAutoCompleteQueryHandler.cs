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
using Core.Application.State.Queries.GetStates;

namespace Core.Application.State.Queries.GetStateAutoComplete
{
    public class GetStateAutoCompleteQueryHandler : IRequestHandler<GetStateAutoCompleteQuery, List<StateDto>>
    
{
     private readonly IDbConnection _dbConnection;

    public GetStateAutoCompleteQueryHandler(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<List<StateDto>> Handle(GetStateAutoCompleteQuery request, CancellationToken cancellationToken)
    {
          var query = @"
            SELECT Id, StateCode, StateName , IsActive,CountryId,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP
            FROM AppData.State with (nolock)
            WHERE StateName   LIKE @SearchPattern OR StateCode  LIKE @SearchPattern AND IsActive = 1
            ORDER BY StateName ";

        // Execute the query and map the result to a list of CountryDto
        var states = await _dbConnection.QueryAsync<StateDto>(
            query, 
            new { SearchPattern = $"%{request.SearchPattern}%" }  // Use the search pattern with wildcards
        );
        if (states == null || !states.Any())
        {
            return new List<StateDto>(); // Return empty list if no matches are found
        }
        return states.AsList();        
    
    }
}
  
}