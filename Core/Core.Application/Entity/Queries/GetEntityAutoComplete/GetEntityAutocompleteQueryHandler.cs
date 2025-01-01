using System.Data;
using Dapper;
using MediatR;
using Core.Application.Entity.Queries.GetEntity;

namespace Core.Application.Entity.Queries.GetEntityAutoComplete
{
    public class GetEntityAutocompleteQueryHandler : IRequestHandler<GetEntityAutocompleteQuery, List<EntityDto>>
    {
    private readonly IDbConnection _dbConnection;
    public GetEntityAutocompleteQueryHandler(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<List<EntityDto>> Handle(GetEntityAutocompleteQuery request, CancellationToken cancellationToken)
    {
        var query = @"
            SELECT *
            FROM AppData.Entity
            WHERE EntityName LIKE @SearchPattern OR EntityCode LIKE @SearchPattern and IsActive = 1
            ORDER BY EntityName";
       // Execute the query and map the result to a list of CountryDto
        var entities = await _dbConnection.QueryAsync<EntityDto>(
            query, 
            new { SearchPattern = $"%{request.SearchPattern}%" }  // Use the search pattern with wildcards
        );
        if (entities == null || !entities.Any())
        {
            return new List<EntityDto>(); // Return empty list if no matches are found
        }

        // Map the results to DTOs
        return entities.Select(entities => new EntityDto
        {
            Id = entities.Id,
            EntityCode = entities.EntityCode,
            EntityName = entities.EntityName,
            IsActive = entities.IsActive,
            EntityDescription = entities.EntityDescription,
            Address = entities.Address,
            Phone = entities.Phone,
            Email = entities.Email
        }).ToList();  
    }
    }
    
}