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
                FROM AppData.City WHERE  IsDeleted=0 ORDER BY ID DESC";
             return (await _dbConnection.QueryAsync<Cities>(query)).ToList();     
        }
        public async Task<Cities> GetByIdAsync(int id)
        { 
            const string query = @"
                SELECT Id, CityCode, CityName, IsActive,StateId,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP 
                FROM AppData.City  WHERE Id = @Id AND IsDeleted=0";
            var city = await _dbConnection.QueryFirstOrDefaultAsync<Cities>(query, new { id });           
            if (city is null)
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
                SELECT Id, CityCode, CityName,StateId, IsActive, CreatedBy, CreatedAt, CreatedByName, CreatedIP, 
                ModifiedBy, ModifiedAt, ModifiedByName, ModifiedIP
                FROM AppData.City
                WHERE (CityName LIKE @SearchPattern OR CityCode LIKE @SearchPattern) 
                AND  IsDeleted=0
                ORDER BY ID DESC";            
            var result = await _dbConnection.QueryAsync<Cities>(query, new { SearchPattern = $"%{searchPattern}%" });
            return result.ToList();
        }

        public async Task<List<Cities>> GetCityByStateIdAsync(int stateId )
        {
           const string query = @"
                SELECT Id, CityCode, CityName,StateId, IsActive, CreatedBy, CreatedAt, CreatedByName, CreatedIP, 
                ModifiedBy, ModifiedAt, ModifiedByName, ModifiedIP
                FROM AppData.City WHERE StateId = @StateId  and IsDeleted=0";
            var result = await _dbConnection.QueryAsync<Cities>(query, new { stateId });           
            if (result is null)
            {
                throw new KeyNotFoundException($"State with ID {stateId} not found.");
            }
            return result.ToList();
        }
    
    }
}