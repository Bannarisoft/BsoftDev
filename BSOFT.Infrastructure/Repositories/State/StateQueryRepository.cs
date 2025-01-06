using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
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

        public async Task<Result<List<States>>> GetByStateNameAsync(string searchPattern)
        {
            if (string.IsNullOrWhiteSpace(searchPattern))
            {
                return Result<List<States>>.Failure("City name cannot be null or empty.");
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

            if (states == null || !states.Any())
            {
                return Result<List<States>>.Failure("No States found.");
            }

            return Result<List<States>>.Success(states.ToList());        
        }
   }
}
