using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.ILanguage;
using Dapper;

namespace UserManagement.Infrastructure.Repositories.Language
{
    public class LanguageQueryRepository : ILanguageQuery
    {
        private readonly IDbConnection _dbConnection;
        public LanguageQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<List<Core.Domain.Entities.Language>> GetAllLanguageAsync()
        {
             const string query = @"
            SELECT 
                Id, 
                Code,
                Name,
                IsActive
            FROM AppData.Language WHERE IsDeleted = 0";
            return (await _dbConnection.QueryAsync<Core.Domain.Entities.Language>(query)).ToList();
        }

        public async Task<Core.Domain.Entities.Language> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM AppData.Language WHERE Id = @Id AND IsDeleted = 0";
            return await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.Language>(query, new { id });
        }

        public async Task<Core.Domain.Entities.Language?> GetByLanguagenameAsync(string name, int? id = null)
        {
             if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("LanguageName cannot be null or empty.", nameof(name));
            }


             var query = """
                 SELECT * FROM AppData.Language 
                 WHERE Name = @Name AND IsDeleted = 0
                 """;

             var parameters = new DynamicParameters(new { Name = name });

             if (id is not null)
             {
                 query += " AND Id != @Id";
                 parameters.Add("Id", id);
             }

            return await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.Language>(query, parameters);
            
        }

        public async Task<List<Core.Domain.Entities.Language>> GetLanguage(string searchPattern)
        {
             if (string.IsNullOrWhiteSpace(searchPattern))
            {
                throw new ArgumentException("LanguageName cannot be null or empty.", nameof(searchPattern));
            }

            const string query = @"
                SELECT Id, Name 
                FROM AppData.Language 
                WHERE IsDeleted = 0 AND Name LIKE @SearchPattern";
                
            var languages = await _dbConnection.QueryAsync<Core.Domain.Entities.Language>(query, new { SearchPattern = $"%{searchPattern}%" });
            return languages.ToList();
        }
    }
}