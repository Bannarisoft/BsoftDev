using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Data;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IEntity;
using System.Data;
using Dapper;
using Core.Application.Common.Interfaces.ICurrency;

namespace UserManagement.Infrastructure.Repositories.Currency
{
    public class CurrencyQueryRepository : ICurrencyQueryRepository
    {
        private readonly IDbConnection _dbConnection; 
        public CurrencyQueryRepository(IDbConnection dbConnection)
        {
             _dbConnection = dbConnection;
        }

        public async Task<List<Core.Domain.Entities.Currency>> GetByIdAsync(int id)
        {
             const string query = @"
               SELECT * 
               FROM AppData.Currency 
               WHERE Id = @Id and IsDeleted = 0
               ORDER BY Id DESC";
             var currencyList = await _dbConnection.QueryAsync<Core.Domain.Entities.Currency>(query, new { id });
             return currencyList?.ToList() ?? new List<Core.Domain.Entities.Currency>();
        }
         public async Task<List<Core.Domain.Entities.Currency>> GetByCurrencyNameAsync(string searchPattern)
        {
          if (string.IsNullOrWhiteSpace(searchPattern))
            {
                throw new ArgumentException("CurrencyName cannot be null or empty.", nameof(searchPattern));
            }

            const string query = @"
                 SELECT *
                 FROM AppData.Currency
                 WHERE Name LIKE @SearchPattern OR Code LIKE @SearchPattern and IsDeleted = 0
                 ORDER BY Id DESC";
                
            // Update the object to use SearchPattern instead of Name
            var Currencylist = await _dbConnection.QueryAsync<Core.Domain.Entities.Currency>(query, new { SearchPattern = $"%{searchPattern}%" });
             return Currencylist?.ToList() ?? new List<Core.Domain.Entities.Currency>();     
        }

        public async Task<List<Core.Domain.Entities.Currency>> GetAllCurrencyAsync()
        {          
            const string query = @"
             SELECT * 
             FROM AppData.Currency 
             WHERE IsDeleted = 0 
             ORDER BY Id DESC";
            return (await _dbConnection.QueryAsync<Core.Domain.Entities.Currency>(query)).ToList() ?? new List<Core.Domain.Entities.Currency>();
        }

    }
}