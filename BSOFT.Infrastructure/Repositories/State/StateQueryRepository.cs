using Core.Domain.Entities;
using System.Data;
using Dapper;
using Core.Application.Common;
using Core.Application.Common.Interfaces.IState;

namespace BSOFT.Infrastructure.Repositories
{    
    public class StateQueryRepository : IStateQueryRepository
    {        
        private readonly IDbConnection _dbConnection;

        public StateQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<States>> GetAllStatesAsync()
        {
             const string query = @"
                SELECT 
                Id,StateCode,StateName,IsActive,CountryId,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP
                FROM AppData.State  where IsDeleted=0 ORDER BY ID DESC";
             return (await _dbConnection.QueryAsync<States>(query)).ToList();     
        }

        public async Task<States> GetByIdAsync(int id)
        {
            const string query = @"
            SELECT Id, StateCode, StateName, IsActive,countryId,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP 
            FROM AppData.State WHERE Id = @Id and IsDeleted=0";
            var state = await _dbConnection.QueryFirstOrDefaultAsync<States>(query, new { id });           
            if (state is null)
            {
                throw new KeyNotFoundException($"State with ID {id} not found.");
            }
            return state;
        }

        public async Task<List<States>> GetByStateNameAsync(string searchPattern)
        {
            if (string.IsNullOrWhiteSpace(searchPattern))
            {                
                throw new ArgumentException("State name cannot be null or empty.", nameof(searchPattern));
            }           
            const string query = @"
                SELECT Id, StateCode, StateName,countryId, IsActive, CreatedBy, CreatedAt, CreatedByName, CreatedIP, 
                ModifiedBy, ModifiedAt, ModifiedByName, ModifiedIP
                FROM AppData.State 
                WHERE (StateName LIKE @SearchPattern OR StateCode LIKE @SearchPattern) 
                AND IsDeleted = 0
                ORDER BY ID DESC";

       /*      var states = await _dbConnection.QueryAsync<States>(
                query,
                new { SearchPattern = $"%{searchPattern}%" }
            ); */
            var result = await _dbConnection.QueryAsync<States>(query, new { SearchPattern = $"%{searchPattern}%" });
            return result.ToList();              
        }
        public async Task<List<States>> GetStateByCountryIdAsync(int countryId )
        {
           const string query = @"
            SELECT Id, StateCode, StateName, IsActive,countryId,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP 
            FROM AppData.State WHERE CountryId = @CountryId  AND IsDeleted=0";
            var state = await _dbConnection.QueryAsync<States>(query, new { countryId });           
            if (state is null)
            {
                throw new KeyNotFoundException($"State with ID {countryId} not found.");
            }
            return state.ToList();
        }
   }
}
