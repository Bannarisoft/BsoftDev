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
                FROM AppData.City where  IsDeleted=0 ORDER BY ID DESC";
             return (await _dbConnection.QueryAsync<Cities>(query)).ToList();     
        }
        public async Task<Cities> GetByIdAsync(int id)
        { 
            const string query = @"
                SELECT Id, CityCode, CityName, IsActive,StateId,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP 
                FROM AppData.City  WHERE Id = @Id and IsDeleted=0";
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
                SELECT Id, CityCode, CityName,StateId, IsActive, CreatedBy, CreatedAt, CreatedByName, CreatedIP, 
                ModifiedBy, ModifiedAt, ModifiedByName, ModifiedIP
                FROM AppData.City
                WHERE (CityName LIKE @SearchPattern OR CityCode LIKE @SearchPattern) 
                AND IsActive = 1 and IsDeleted=0
                ORDER BY ID DESC";            
            var result = await _dbConnection.QueryAsync<Cities>(query, new { SearchPattern = $"%{searchPattern}%" });
            return result.ToList();
        }
    }
}