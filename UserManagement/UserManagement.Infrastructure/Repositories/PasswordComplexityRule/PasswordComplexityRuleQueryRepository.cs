using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IPasswordComplexityRule;
using Core.Domain.Entities;
using Dapper;
using DnsClient.Internal;



namespace UserManagement.Infrastructure.Repositories.PasswordComplexityRule
{
    public class PasswordComplexityRuleQueryRepository : IPasswordComplexityRuleQueryRepository
    { 
          private readonly IDbConnection _dbConnection; 
    public  PasswordComplexityRuleQueryRepository(IDbConnection dbConnection)
    {
         _dbConnection = dbConnection;
    }

      public async Task<List<Core.Domain.Entities.PasswordComplexityRule>>GetPasswordComplexityAsync( )
    {
        
        const string query = @"SELECT  * FROM AppSecurity.PasswordComplexityRule WHERE IsDeleted = 0 ORDER BY Id DESC";
            return (await _dbConnection.QueryAsync<Core.Domain.Entities.PasswordComplexityRule>(query)).ToList();        
    }

      public async Task<Core.Domain.Entities.PasswordComplexityRule> GetByIdAsync(int id)
    {
       

         const string query = @"SELECT * FROM AppSecurity.PasswordComplexityRule WHERE Id = @Id AND IsDeleted = 0 ORDER BY Id DESC";
            var passwordComplexity = await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.PasswordComplexityRule>(query, new { id });           
             if (passwordComplexity == null)
            {
               // throw new KeyNotFoundException($" PasswordComplexityRule with ID {id} not found."); 
               return null;            
               
            }
            return passwordComplexity;
        }       
         public async Task<List<Core.Domain.Entities.PasswordComplexityRule>>  GetpwdautocompleteAsync(string searchTerm = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                throw new ArgumentException("PwdComplexityRule cannot be null or empty.", nameof(searchTerm));
            }
           const string query = @" 
           SELECT  * FROM  AppSecurity.PasswordComplexityRule WHERE PwdComplexityRule LIKE @searchTerm OR Id LIKE @searchTerm AND IsDeleted = 0
            ORDER BY ID DESC";
            // Update the object to use SearchPattern instead of Name
          var Pwdrule = await _dbConnection.QueryAsync<Core.Domain.Entities.PasswordComplexityRule>(query, new { SearchTerm  = $"%{searchTerm}%" });
            return Pwdrule.ToList();
        }
    }

   
}