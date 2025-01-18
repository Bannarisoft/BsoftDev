using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IPasswordComplexityRule;
using System.Data;
using Dapper;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;

namespace BSOFT.Infrastructure.Repositories.PasswordComplexityRule
{
    public class PasswordComplexityRuleQueryRepository   : IPasswordComplexityRuleQueryRepository
    { 
         private readonly IDbConnection _dbConnection; 
    public  PasswordComplexityRuleQueryRepository(IDbConnection dbConnection)
    {
         _dbConnection = dbConnection;
    }

      public async Task<List<Core.Domain.Entities.PasswordComplexityRule>>GetPasswordComplexityAsync()
    {
        
        const string query = @"SELECT  * FROM AppSecurity.PasswordComplexityRule";
            return (await _dbConnection.QueryAsync<Core.Domain.Entities.PasswordComplexityRule>(query)).ToList();        
    }

      public async Task<Core.Domain.Entities.PasswordComplexityRule> GetByIdAsync(int id)
    {
         //  const string query = "SELECT * FROM AppData.Department WHERE Id = @Id";
      //  return await _dbConnection.QueryFirstOrDefaultAsync<Department>(query, new { id });

         const string query = @"SELECT * FROM AppSecurity.PasswordComplexityRule WHERE Id = @Id ";
            var passwordComplexity = await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.PasswordComplexityRule>(query, new { id });           
             if (passwordComplexity == null)
            {
                throw new KeyNotFoundException($" PasswordComplexityRule with ID {id} not found.");
            }
            return passwordComplexity;
        }   
    




    



        
    }

   
}