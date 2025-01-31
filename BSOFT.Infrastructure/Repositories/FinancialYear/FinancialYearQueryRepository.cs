using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IFinancialYear;
using Dapper;

namespace BSOFT.Infrastructure.Repositories.FinancialYear
{
    public class FinancialYearQueryRepository : IFinancialYearQueryRepository
    {        
          private readonly IDbConnection _dbConnection; 

    public  FinancialYearQueryRepository(IDbConnection dbConnection)
    {
         _dbConnection = dbConnection;
    }

     public async Task<List<Core.Domain.Entities.FinancialYear>>GetAllFinancialYearAsync()
    {
        
        const string query = @"SELECT  * FROM AppData.FinancialYear WHERE IsActive = 1 order by CreatedAt desc";
            return (await _dbConnection.QueryAsync<Core.Domain.Entities.FinancialYear>(query)).ToList();
        
    }
       public async Task<Core.Domain.Entities.FinancialYear>GetByIdAsync(int id)
    {
               

             const string query = @"SELECT * FROM AppData.FinancialYear WHERE Id = @Id AND IsActive = 1  order by CreatedAt desc";
            var financialyear = await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.FinancialYear>(query, new { id });           
             if (financialyear == null)
            {
                throw new KeyNotFoundException($"FinancialYear with ID {id} not found.");
            }
            return financialyear;
            
    } 




    }
}