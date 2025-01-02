using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.City.Queries.GetCities;
using Core.Application.City.Queries.GetCityById;
using Dapper;
using MediatR;

namespace Core.Application.Country.Queries.GetCountryById
{
    public class GetCityByIdQueryHandler : IRequestHandler<GetCityByIdQuery, CityDto?>
{
    private readonly IDbConnection _dbConnection;

public GetCityByIdQueryHandler(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<CityDto?> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
    {
        var query = "SELECT Id, CityCode, CityName, IsActive,StateId,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP FROM AppData.City  with(nolock) WHERE Id = @Id and IsActive=1";
        var city = await _dbConnection.QuerySingleOrDefaultAsync<CityDto>(query, new { Id = request.Id });
        
        if (city == null)
        {
            return null;
        }
        return city;
        /* // Map the country entity to a DTO
        return new CityDto
        {
            Id = city.Id,
            CityCode = city.CityCode,
            CityName = city.CityName,
            IsActive = city.IsActive,
            StateId =city.StateId
            
        }; */
    }
}
}