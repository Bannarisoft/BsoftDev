using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Application.Country.DTO;
using BSOFT.Domain.Common.Interface;
using Dapper;
using MediatR;

namespace BSOFT.Application.Country.Queries.GetCountryById
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
        var query = "SELECT Id, countryCode, countryName, IsActive FROM AppData.Country WHERE Id = @Id";
        var country = await _dbConnection.QuerySingleOrDefaultAsync<CountryDto>(query, new { Id = request.Id });
        // Return null if the country is not found
        if (country == null)
        {
            return null;
        }

        // Map the country entity to a DTO
        return new CountryDto
        {
            Id = country.Id,
            CountryCode = country.CountryCode,
            CountryName = country.CountryName,
            IsActive = country.IsActive
        };
    }
}
}