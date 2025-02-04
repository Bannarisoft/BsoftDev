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


        public async Task<List<Entity>> GetByIdAsync(int id)
        {
             const string query = @"
             SELECT * 
             FROM AppData.Entity 
             WHERE Id = @Id and IsDeleted = 0
             ORDER BY CreatedAt DESC";
             var entityList = await _dbConnection.QueryAsync<Entity>(query, new { id });
             return entityList?.ToList() ?? new List<Entity>();
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

           public async Task<List<Entity>> GetByEntityNameAsync(string searchPattern)
        {
          if (string.IsNullOrWhiteSpace(searchPattern))
            {
                throw new ArgumentException("EntityName cannot be null or empty.", nameof(searchPattern));
            }

            const string query = @"
                SELECT *
                FROM AppData.Entity
                WHERE EntityName LIKE @SearchPattern OR EntityCode LIKE @SearchPattern and IsDeleted = 0
                ORDER BY CreatedAt DESC";
                
            // Update the object to use SearchPattern instead of Name
            var Entitylist = await _dbConnection.QueryAsync<Entity>(query, new { SearchPattern = $"%{searchPattern}%" });
             return Entitylist?.ToList() ?? new List<Entity>();     
        }

        public async Task<List<Entity>> GetAllEntityAsync()
        {          
             const string query = @"
              SELECT *
              FROM AppData.Entity WHERE IsDeleted = 0 
              ORDER BY CreatedAt DESC";
            return (await _dbConnection.QueryAsync<Entity>(query)).ToList() ?? new List<Entity>();
        }

        
    }
}