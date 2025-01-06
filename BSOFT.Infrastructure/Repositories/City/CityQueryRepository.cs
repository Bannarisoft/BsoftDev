using Core.Domain.Entities;
using System.Data;
using Dapper;
using Core.Application.Common;
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
                FROM AppData.City with (nolock) where IsActive=1 order by CityName";
             return (await _dbConnection.QueryAsync<Cities>(query)).ToList();     
        }
        public async Task<Cities> GetByIdAsync(int id)
        { 
            const string query = @"
                SELECT Id, CityCode, CityName, IsActive,StateId,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP 
                FROM AppData.City  with(nolock) WHERE Id = @Id and IsActive=1";
            var city = await _dbConnection.QueryFirstOrDefaultAsync<Cities>(query, new { id });           
            if (city == null)
            {
                throw new KeyNotFoundException($"City with ID {id} not found.");
            }

            return city;
        }
        public async Task<Result<List<Cities>>> GetByCityNameAsync(string searchPattern)
        {
           if (string.IsNullOrWhiteSpace(searchPattern))
            {
                return Result<List<Cities>>.Failure("City name cannot be null or empty.");
            }

            const string query = @"
                SELECT Id, CityCode, cityName,stateId, IsActive, CreatedBy, CreatedAt, CreatedByName, CreatedIP, 
                ModifiedBy, ModifiedAt, ModifiedByName, ModifiedIP
                FROM AppData.City WITH (NOLOCK)
                WHERE (cityName LIKE @SearchPattern OR cityName LIKE @SearchPattern) 
                AND IsActive = 1
                ORDER BY cityName";

            var cities = await _dbConnection.QueryAsync<Cities>(
                query,
                new { SearchPattern = $"%{searchPattern}%" }
            );

            if (cities == null || !cities.Any())
            {
                return Result<List<Cities>>.Failure("No Cities found.");
            }
            return Result<List<Cities>>.Success(cities.ToList());
        }
    }
}