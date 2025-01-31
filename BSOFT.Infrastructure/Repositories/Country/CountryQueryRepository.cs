using Core.Domain.Entities;
using System.Data;
using Dapper;
using Core.Application.Common.Interfaces.ICountry;

namespace BSOFT.Infrastructure.Repositories.Country
{    
    public class CountryQueryRepository : ICountryQueryRepository
    {        
        private readonly IDbConnection _dbConnection;
        
        public CountryQueryRepository(IDbConnection dbConnection)
        {            
            _dbConnection = dbConnection;
        }
      
        public async Task<List<Countries>> GetAllCountriesAsync()
        {
            const string query = @"
            SELECT Id,CountryCode, CountryName, IsActive ,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP
            FROM AppData.Country with (nolock) where isdeleted=0 order by Id desc";
            return (await _dbConnection.QueryAsync<Countries>(query)).ToList();
        }

        public async Task<Countries> GetByIdAsync(int id)
        {          

            const string query = @"
            SELECT Id, countryCode, countryName, IsActive, CreatedBy, CreatedAt, CreatedByName, CreatedIP, 
            ModifiedBy, ModifiedAt, ModifiedByName, ModifiedIP
            FROM AppData.Country WITH (NOLOCK)
            WHERE Id = @id AND isdeleted = 0";
            var country = await _dbConnection.QueryFirstOrDefaultAsync<Countries>(query, new { id });           
             if (country == null)
            {
                throw new KeyNotFoundException($"Country with ID {id} not found.");
            }
            return country;
        }
        public async Task<List<Countries>> GetByCountryNameAsync(string searchPattern)
        {
            if (string.IsNullOrWhiteSpace(searchPattern))
            {                
                throw new ArgumentException("Country name cannot be null or empty.", nameof(searchPattern));
            }
            const string query = @"
                SELECT Id, countryCode, countryName, IsActive, CreatedBy, CreatedAt, CreatedByName, CreatedIP, 
                ModifiedBy, ModifiedAt, ModifiedByName, ModifiedIP
                FROM AppData.Country WITH (NOLOCK)
                WHERE (countryName LIKE @SearchPattern OR countryCode LIKE @SearchPattern) 
                AND isdeleted = 0
                ORDER BY id desc";

            var countries = await _dbConnection.QueryAsync<Countries>(
                query,
                new { SearchPattern = $"%{searchPattern}%" }
            );
            var result = await _dbConnection.QueryAsync<Countries>(query, new { SearchPattern = $"%{searchPattern}%" });
            return result.ToList();
        }
    }
}