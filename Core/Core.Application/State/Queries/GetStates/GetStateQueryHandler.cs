using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interface;
using Core.Application.State.Queries.GetStates;
using Dapper;
using MediatR;

namespace Core.Application.State.Queries.GetCountries
{
    public class GetStateQueryHandler : IRequestHandler<GetStateQuery, List<StateDto>>
{
    private readonly IDbConnection _dbConnection;

    public GetStateQueryHandler(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    public async Task<List<StateDto>> Handle(GetStateQuery request, CancellationToken cancellationToken)
    {
         const string query = @"
            SELECT 
                Id,StateCode,StateName, IsActive,countryId ,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP
            FROM AppData.State with (nolock) where IsActive=1";

        var countries = await _dbConnection.QueryAsync<StateDto>(query);
        return countries.AsList();
    }
}
}