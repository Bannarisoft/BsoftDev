using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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