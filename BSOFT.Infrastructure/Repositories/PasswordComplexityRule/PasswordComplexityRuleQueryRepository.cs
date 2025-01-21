using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IPasswordComplexityRule;
using Core.Domain.Entities;
using Dapper;



namespace BSOFT.Infrastructure.Repositories.PasswordComplexityRule
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
        
        const string query = @"SELECT  * FROM AppSecurity.PasswordComplexityRule";
            return (await _dbConnection.QueryAsync<Core.Domain.Entities.PasswordComplexityRule>(query)).ToList();        
    }

      public async Task<Core.Domain.Entities.PasswordComplexityRule> GetByIdAsync(int id)
    {
       

         const string query = @"SELECT * FROM AppSecurity.PasswordComplexityRule WHERE Id = @Id ";
            var passwordComplexity = await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.PasswordComplexityRule>(query, new { id });           
             if (passwordComplexity == null)
            {
                throw new KeyNotFoundException($" PasswordComplexityRule with ID {id} not found.");
            }
            return passwordComplexity;
        }

        // public Task<List<Core.Domain.Entities.PasswordComplexityRule>> GetpwdautocompleteAsync(string searchTerm)
        // {
        //        if (string.IsNullOrWhiteSpace(searchTerm))
        //     {
        //         throw new ArgumentException("PasswordComplexityRule cannot be null or empty.", nameof(searchTerm));
        //     }

        //     const string query = @"
        //          select Id,PwdComplexityRule from  AppSecurity.PasswordComplexityRule 
        //     WHERE PwdComplexityRule LIKE @searchTerm OR Id LIKE @searchTerm and IsActive =1
        //     ORDER BY PwdComplexityRule";

            
        //     var result = await _dbConnection.QueryAsync<Core.Domain.Entities.PasswordComplexityRule>(query, new { SearchTerm = $"%{searchTerm}%" });

        //     return result.ToList();



            
        // }

         public async Task<List<Core.Domain.Entities.PasswordComplexityRule>>  GetpwdautocompleteAsync(string searchTerm = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                throw new ArgumentException("PwdComplexityRule cannot be null or empty.", nameof(searchTerm));
            }
           const string query = @" 
           select  * from  AppSecurity.PasswordComplexityRule 
            WHERE PwdComplexityRule LIKE @searchTerm OR Id LIKE @searchTerm and IsActive =1
            ORDER BY PwdComplexityRule";
            // Update the object to use SearchPattern instead of Name
          var Pwdrule = await _dbConnection.QueryAsync<Core.Domain.Entities.PasswordComplexityRule>(query, new { SearchTerm  = $"%{searchTerm}%" });
            return Pwdrule.ToList();
        }
    }

   
}