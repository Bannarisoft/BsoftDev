using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Country.Queries.GetCountries;
using Core.Application.Country.Queries.GetCountryById;
using Core.Application.Common.Interface;
using Dapper;
using MediatR;
using Core.Application.Country.Queries.GetcountryById;

namespace Core.Application.Country.Queries.GetCountryById
{
    public class GetCountryByIdQueryHandler : IRequestHandler<GetCountryByIdQuery, CountryDto?>
{
    private readonly IDbConnection _dbConnection;

public GetCountryByIdQueryHandler(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<CountryDto?> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
    {
        var query = "SELECT Id, countryCode, countryName, IsActive,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP FROM AppData.Country with (nolock) WHERE  Id = @Id and isActive=1";
        var country = await _dbConnection.QuerySingleOrDefaultAsync<CountryDto>(query, new { Id = request.Id });
        // Return null if the country is not found
        if (country == null)
        {
            return null;
        }

       return country;
    }
}
}