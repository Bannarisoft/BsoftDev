using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using System.Data;
using Dapper;
using Core.Application.Common;
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
            FROM AppData.Country with (nolock) where isActive=1";
            return (await _dbConnection.QueryAsync<Countries>(query)).ToList();
        }

        public async Task<Countries> GetByIdAsync(int id)
        {          

            const string query = @"
            SELECT Id, countryCode, countryName, IsActive, CreatedBy, CreatedAt, CreatedByName, CreatedIP, 
            ModifiedBy, ModifiedAt, ModifiedByName, ModifiedIP
            FROM AppData.Country WITH (NOLOCK)
            WHERE Id = @id AND IsActive = 1";
            var country = await _dbConnection.QueryFirstOrDefaultAsync<Countries>(query, new { id });           
             if (country == null)
            {
                throw new KeyNotFoundException($"Country with ID {id} not found.");
            }
            return country;
        }
        public async Task<Result<List<Countries>>> GetByCountryNameAsync(string countryName)
        {
            if (string.IsNullOrWhiteSpace(countryName))
            {
                return Result<List<Countries>>.Failure("Country name cannot be null or empty.");
            }

            const string query = @"
                SELECT Id, countryCode, countryName, IsActive, CreatedBy, CreatedAt, CreatedByName, CreatedIP, 
                ModifiedBy, ModifiedAt, ModifiedByName, ModifiedIP
                FROM AppData.Country WITH (NOLOCK)
                WHERE (countryName LIKE @SearchPattern OR countryCode LIKE @SearchPattern) 
                AND IsActive = 1
                ORDER BY countryName";

            var countries = await _dbConnection.QueryAsync<Countries>(
                query,
                new { SearchPattern = $"%{countryName}%" }
            );

            if (countries == null || !countries.Any())
            {
                return Result<List<Countries>>.Failure("No countries found.");
            }
            return Result<List<Countries>>.Success(countries.ToList());
        }
    }
}