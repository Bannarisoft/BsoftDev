using Core.Application.Entity.Queries.GetEntity;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace Core.Application.Entity.Queries.GetEntityById
{
    public class GetEntityByIdQueryHandler : IRequestHandler<GetEntityByIdQuery, EntityDto?>
    {
    private readonly IEntityRepository _entityRepository;
    private readonly IDbConnection _dbConnection;

    public GetEntityByIdQueryHandler(IDbConnection dbConnection)
    {
            _dbConnection = dbConnection;
    }

    public async Task<EntityDto?> Handle(GetEntityByIdQuery request, CancellationToken cancellationToken)
    {
        var query = "SELECT * FROM AppData.Entity WHERE Id = @Id";
        var entityresult = await _dbConnection.QuerySingleOrDefaultAsync<EntityDto>(query, new { Id = request.EntityId });
        // Return null if the country is not found
        if (entityresult == null)
        {
            return null;
        }
        // Map the entity to a DTO
        return new EntityDto
        {
            Id = entityresult.Id,
            EntityCode = entityresult.EntityCode,
            EntityName = entityresult.EntityName,
            EntityDescription = entityresult.EntityDescription,
            Address= entityresult.Address,
            Phone= entityresult.Phone,
            Email= entityresult.Email,
            IsActive = entityresult.IsActive
        };
    }
    }
}