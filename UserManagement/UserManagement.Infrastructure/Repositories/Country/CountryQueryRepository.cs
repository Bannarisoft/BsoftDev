using Core.Domain.Entities;
using System.Data;
using Dapper;
using Core.Application.Common.Interfaces.ICountry;

namespace UserManagement.Infrastructure.Repositories.Country
{    
    public class CountryQueryRepository : ICountryQueryRepository
    {        
        private readonly IDbConnection _dbConnection;
        
        public CountryQueryRepository(IDbConnection dbConnection)
        {            
            _dbConnection = dbConnection;
        }
        public async Task<(List<Countries>, int)> GetAllCountriesAsync(int PageNumber, int PageSize, string? SearchTerm)
        {

               var query = $$"""
                DECLARE @TotalCount INT;
                SELECT @TotalCount = COUNT(*) 
                FROM AppData.Country 
                WHERE IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CountryCode LIKE @Search OR CountryName LIKE @Search)")}};

                SELECT Id,CountryCode, CountryName, IsActive ,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP
                FROM AppData.Country WHERE IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CountryCode LIKE @Search OR CountryName LIKE @Search )")}}
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

          
            var countries = await _dbConnection.QueryMultipleAsync(query, parameters);
            var countryList = (await countries.ReadAsync<Countries>()).ToList();
            int totalCount = (await countries.ReadFirstAsync<int>());             
            return (countryList, totalCount);    
        }

        public async Task<Countries> GetByIdAsync(int id)
        {          

            const string query = @"
            SELECT Id, CountryCode, CountryName, IsActive, CreatedBy, CreatedAt, CreatedByName, CreatedIP, 
            ModifiedBy, ModifiedAt, ModifiedByName, ModifiedIP
            FROM AppData.Country
            WHERE Id = @id AND IsDeleted = 0";
            var country = await _dbConnection.QueryFirstOrDefaultAsync<Countries>(query, new { id });           
             if (country is null)
            {
                throw new KeyNotFoundException($"Country with ID {id} not found.");
            }
            return country;
        }
        public async Task<List<Countries>> GetByCountryNameAsync(string searchPattern)
        {           
            const string query = @"
                SELECT Id, countryCode, countryName, IsActive, CreatedBy, CreatedAt, CreatedByName, CreatedIP, 
                ModifiedBy, ModifiedAt, ModifiedByName, ModifiedIP
                FROM AppData.Country
                WHERE (CountryName LIKE @SearchPattern OR CountryCode LIKE @SearchPattern) 
                AND IsDeleted = 0 and IsActive=1
                ORDER BY ID DESC";
            var result = await _dbConnection.QueryAsync<Countries>(query, new { SearchPattern = $"%{searchPattern}%" });
            return result.ToList();
        }
    }
}