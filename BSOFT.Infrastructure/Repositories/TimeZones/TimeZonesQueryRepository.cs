using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.ITimeZones;
using Dapper;

namespace BSOFT.Infrastructure.Repositories.TimeZones
{
    public class TimeZonesQueryRepository : ITimeZonesQueryRepository
    {
        private readonly IDbConnection _dbConnection; 
        public TimeZonesQueryRepository(IDbConnection dbConnection)
        {
             _dbConnection = dbConnection;
        }
        public async Task<List<Core.Domain.Entities.TimeZones>> GetByIdAsync(int id)
        {
             const string query = "SELECT * FROM AppData.Timezones WHERE Id = @Id and IsActive = 1";
             var timeszoneList =await _dbConnection.QueryAsync<Core.Domain.Entities.TimeZones>(query, new { id });
             return timeszoneList?.ToList() ?? new List<Core.Domain.Entities.TimeZones>();
        }

        public async Task<List<Core.Domain.Entities.TimeZones>> GetAllTimeZonesAsync()
        {
             const string query = @"
              SELECT *
            FROM AppData.Timezones Where IsActive = 1 ORDER BY CreatedAt DESC";
            return (await _dbConnection.QueryAsync<Core.Domain.Entities.TimeZones>(query)).ToList() ?? new List<Core.Domain.Entities.TimeZones>();
        }

     public async Task<List<Core.Domain.Entities.TimeZones>> GetByTimeZonesNameAsync(string searchPattern)
        {
          if (string.IsNullOrWhiteSpace(searchPattern))
            {
                throw new ArgumentException("CurrencyName cannot be null or empty.", nameof(searchPattern));
            }

            const string query = @"
                 SELECT *
            FROM AppData.Timezones
            WHERE Name LIKE @SearchPattern OR Code LIKE @SearchPattern and IsActive = 1
            ORDER BY CreatedAt DESC";
                
            // Update the object to use SearchPattern instead of Name
            var timeszoneList = await _dbConnection.QueryAsync<Core.Domain.Entities.TimeZones>(query, new { SearchPattern = $"%{searchPattern}%" });
            return timeszoneList?.ToList() ?? new List<Core.Domain.Entities.TimeZones>();     
        }

        
    }
}