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
                FROM AppData.State with (nolock) where IsActive=1 order by StateName";
             return (await _dbConnection.QueryAsync<States>(query)).ToList();     
        }

        public async Task<States> GetByIdAsync(int id)
        {
            const string query = @"
            SELECT Id, StateCode, StateName, IsActive,countryId,CreatedBy,CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedAt,ModifiedByName,ModifiedIP 
            FROM AppData.State with(nolock) WHERE Id = @Id and IsActive=1";
            var state = await _dbConnection.QueryFirstOrDefaultAsync<States>(query, new { id });           
            if (state == null)
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
                FROM AppData.State WITH (NOLOCK)
                WHERE (StateName LIKE @SearchPattern OR StateCode LIKE @SearchPattern) 
                AND IsActive = 1
                ORDER BY StateName";

            var states = await _dbConnection.QueryAsync<States>(
                query,
                new { SearchPattern = $"%{searchPattern}%" }
            );
            var result = await _dbConnection.QueryAsync<States>(query, new { SearchPattern = $"%{searchPattern}%" });
            return result.ToList();              
        }
   }
}
