using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IEntity;
using System.Data;
using Dapper;


namespace BSOFT.Infrastructure.Repositories.Entities
{
    public class EntityQueryRepository : IEntityQueryRepository
    {
        private readonly IDbConnection _dbConnection;        

        public EntityQueryRepository(IDbConnection dbConnection)
        {
             _dbConnection = dbConnection;
        }

        public async Task<List<Entity>> GetAllEntityAsync()
        {          
             const string query = @"
              SELECT *
            FROM AppData.Entity";
            return (await _dbConnection.QueryAsync<Entity>(query)).ToList();
        }

        public async Task<Entity> GetByIdAsync(int id)
        {
          
             const string query = "SELECT * FROM AppData.Entity WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Entity>(query, new { id });
        }  
        public async Task<List<Entity>> GetByEntityNameAsync(string searchPattern)
        {
          if (string.IsNullOrWhiteSpace(searchPattern))
            {
                throw new ArgumentException("DivisionName cannot be null or empty.", nameof(searchPattern));
            }

            const string query = @"
                 SELECT *
            FROM AppData.Entity
            WHERE EntityName LIKE @SearchPattern OR EntityCode LIKE @SearchPattern and IsActive = 1
            ORDER BY EntityName";
                
            // Update the object to use SearchPattern instead of Name
            var divisions = await _dbConnection.QueryAsync<Entity>(query, new { SearchPattern = $"%{searchPattern}%" });
            return divisions.ToList();       
        }

        
    }
}