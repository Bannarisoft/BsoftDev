using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Data;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IEntity;
using System.Data;
using Dapper;
using Core.Application.Entity.Queries.GetEntityLastCode;
using Microsoft.Data.SqlClient;

namespace UserManagement.Infrastructure.Repositories.Entities
{
    public class EntityQueryRepository : IEntityQueryRepository
    {
        private readonly IDbConnection _dbConnection;        

        public EntityQueryRepository(IDbConnection dbConnection)
        {
             _dbConnection = dbConnection;
        }

        public async Task<string> GenerateEntityCodeAsync()
        {
            var query = @"
            SELECT TOP 1 EntityCode 
            FROM AppData.Entity 
            ORDER BY Id DESC";
            var lastCode = await _dbConnection.QueryFirstOrDefaultAsync<string>(query);

            if (string.IsNullOrEmpty(lastCode))
            {
              lastCode = "ENT-00000";
            }

            var nextCodeNumber = int.Parse(lastCode[(lastCode.IndexOf('-') + 1)..]) + 1;

            return $"ENT-{nextCodeNumber:D5}"; 
        }

        public async Task<(List<Entity>, int)> GetAllEntityAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
           var query = $$"""
             DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
               FROM AppData.Entity
              WHERE IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (EntityName LIKE @Search OR EntityCode LIKE @Search)")}};

                SELECT 
                Id, 
                EntityCode,
                EntityName,
                EntityDescription,
                Address,
                Phone,
                Email,
                IsActive
            FROM AppData.Entity 
            WHERE 
            IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (EntityName LIKE @Search OR EntityCode LIKE @Search )")}}
                ORDER BY Id desc
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                SELECT @TotalCount AS TotalCount;
            """;

            
             var parameters = new
                       {
                           Search = $"%{SearchTerm}%",
                           Offset = (PageNumber - 1) * PageSize,
                           PageSize
                       };

             var entitygroup = await _dbConnection.QueryMultipleAsync(query, parameters);
             var entitiesgrouplist = (await entitygroup.ReadAsync<Core.Domain.Entities.Entity>()).ToList();
             int totalCount = (await entitygroup.ReadFirstAsync<int>());
             return (entitiesgrouplist, totalCount);
        }

        public async Task<List<Entity>> GetByEntityNameAsync(string searchPattern)
        {
            searchPattern = searchPattern ?? string.Empty; // Prevent null issues

            const string query = @"
             SELECT Id, EntityName 
            FROM AppData.Entity
            WHERE IsDeleted = 0 
            AND EntityName LIKE @SearchPattern";  
            var parameters = new 
            { 
            SearchPattern = $"%{searchPattern}%" 
            };

            var entitiesGroups = await _dbConnection.QueryAsync<Core.Domain.Entities.Entity>(query, parameters);
            return entitiesGroups.ToList();  
        }

        public async Task<Entity> GetByIdAsync(int Id)
        {
              const string query = @"
                    SELECT * 
                    FROM AppData.Entity 
                    WHERE Id = @Id AND IsDeleted = 0";
                    var entityGroup = await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.Entity>(query, new { Id });
                    return entityGroup;
        }

        


    }
}