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

    



        
    }

   
}