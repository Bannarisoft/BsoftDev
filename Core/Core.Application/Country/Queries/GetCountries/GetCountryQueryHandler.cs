using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Country.Queries.GetCountries;
using Core.Application.Common.Interface;
using Dapper;
using MediatR;

namespace Core.Application.Country.Queries.GetCountries
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
            SELECT Id,CountryCode, CountryName, IsActive ,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP
            FROM AppData.Country with (nolock) where isActive=1";

        var countries = await _dbConnection.QueryAsync<CountryDto>(query);
        return countries.AsList();
    }
}
}