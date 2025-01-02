using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interface;
using Dapper;
using MediatR;
using Core.Application.State.Queries.GetStateById;
using Core.Application.State.Queries.GetStates;

namespace Core.Application.State.Queries.GetStateById
{
    public class GetStateByIdQueryHandler : IRequestHandler<GetStateByIdQuery, StateDto?>
{
    private readonly IDbConnection _dbConnection;

public GetStateByIdQueryHandler(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<StateDto?> Handle(GetStateByIdQuery request, CancellationToken cancellationToken)
    {
        var query = "SELECT Id, StateCode, StateName, IsActive,countryId,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP FROM AppData.State with(nolock) WHERE Id = @Id and IsActive=1";
        var state = await _dbConnection.QuerySingleOrDefaultAsync<StateDto>(query, new { Id = request.Id });        
        if (state == null)
        {
            return null;
        }
        return state;
    }
}
}