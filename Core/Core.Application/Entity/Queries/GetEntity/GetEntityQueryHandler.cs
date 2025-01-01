using MediatR;
using Dapper;
using System.Data;


namespace Core.Application.Entity.Queries.GetEntity
{
    public class GetEntityQueryHandler : IRequestHandler<GetEntityQuery, List<EntityDto>>
    {
         private readonly IDbConnection _dbConnection;

        public GetEntityQueryHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<List<EntityDto>> Handle(GetEntityQuery request, CancellationToken cancellationToken)
        {
            const string query = @"
            SELECT *
            FROM AppData.Entity";
        var Entities = await _dbConnection.QueryAsync<EntityDto>(query);
        return Entities.AsList();
        }
    }
}