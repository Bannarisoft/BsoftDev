using Core.Domain.Entities;
using System.Data;
using Dapper;
using Core.Application.Common.Interfaces.ICity;

namespace BSOFT.Infrastructure.Repositories.City
{
    public class CityQueryRepository : ICityQueryRepository
    {        
        private readonly IDbConnection _dbConnection;
        public CityQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }     
        public async Task<List<Cities>> GetAllCityAsync()
        {
            const string query = @"
                SELECT 
                Id,CityCode,CityName,IsActive,StateId,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP
                FROM AppData.City with (nolock) where  isdeleted=0 order by Id desc";
             return (await _dbConnection.QueryAsync<Cities>(query)).ToList();     
        }
        public async Task<Cities> GetByIdAsync(int id)
        { 
            const string query = @"
                SELECT Id, CityCode, CityName, IsActive,StateId,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP 
                FROM AppData.City  with(nolock) WHERE Id = @Id and isdeleted=0";
            var city = await _dbConnection.QueryFirstOrDefaultAsync<Cities>(query, new { id });           
            if (city == null)
            {
                throw new KeyNotFoundException($"City with ID {id} not found.");
            }
            return city;
        }
        public async Task<List<Cities>> GetByCityNameAsync(string searchPattern)
        {
           if (string.IsNullOrWhiteSpace(searchPattern))
            {                
                throw new ArgumentException("City name cannot be null or empty.", nameof(searchPattern));
            }
            const string query = @"
                SELECT Id, CityCode, cityName,stateId, IsActive, CreatedBy, CreatedAt, CreatedByName, CreatedIP, 
                ModifiedBy, ModifiedAt, ModifiedByName, ModifiedIP
                FROM AppData.City WITH (NOLOCK)
                WHERE (cityName LIKE @SearchPattern OR cityName LIKE @SearchPattern) 
                AND IsActive = 1 and isdeleted=0
                ORDER BY Id desc";            
            var result = await _dbConnection.QueryAsync<Cities>(query, new { SearchPattern = $"%{searchPattern}%" });
            return result.ToList();
        }
    }
}