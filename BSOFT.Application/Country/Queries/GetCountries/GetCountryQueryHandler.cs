using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Application.Country.DTO;
using BSOFT.Domain.Common.Interface;
using Dapper;
using MediatR;

namespace BSOFT.Application.Country.Queries.GetCountries
{
    public class GetCountryQueryHandler : IRequestHandler<GetCountryQuery, List<CountryDto>>
{
    private readonly IDbConnection _dbConnection;

    public GetCountryQueryHandler(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    public async Task<List<CountryDto>> Handle(GetCountryQuery request, CancellationToken cancellationToken)
    {
         const string query = @"
            SELECT 
                Id, 
                countryCode, 
                countryName, 
                IsActive 
            FROM AppData.Country";

        var countries = await _dbConnection.QueryAsync<CountryDto>(query);
        return countries.AsList();
    }
}
}