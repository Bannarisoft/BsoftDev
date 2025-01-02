using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Country.Queries.GetCountries;
using Core.Application.Common.Interface;
using Dapper;
using MediatR;

namespace Core.Application.City.Queries.GetCities
{
    public class GetCityQueryHandler : IRequestHandler<GetCityQuery, List<CityDto>>
{
    private readonly IDbConnection _dbConnection;

    public GetCityQueryHandler(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    public async Task<List<CityDto>> Handle(GetCityQuery request, CancellationToken cancellationToken)
    {
         const string query = @"
            SELECT 
            Id,CityCode,CityName,IsActive,StateId,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP
            FROM AppData.City with (nolock) where IsActive=1";

        var cities = await _dbConnection.QueryAsync<CityDto>(query);     
        return cities.AsList();
    }
}
}