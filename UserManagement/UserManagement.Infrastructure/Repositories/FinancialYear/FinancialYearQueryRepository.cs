using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IFinancialYear;
using Dapper;

namespace UserManagement.Infrastructure.Repositories.FinancialYear
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
        
        const string query = @"SELECT  * FROM AppData.FinancialYear WHERE   IsDeleted = 0 ORDER BY ID DESC ";
        return (await _dbConnection.QueryAsync<Core.Domain.Entities.FinancialYear>(query)).ToList();
        
    }
       public async Task<Core.Domain.Entities.FinancialYear>GetByIdAsync(int id)
    {               
             const string query = @"SELECT * FROM AppData.FinancialYear WHERE Id = @Id AND   IsDeleted = 0  ORDER BY ID DESC";
            return await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.FinancialYear>(query, new { id });
    } 

     public async Task<List<Core.Domain.Entities.FinancialYear>>  GetAllFinancialAutoCompleteSearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                throw new ArgumentException($"FinancialYear start year { nameof(searchTerm)} cannot be null or empty.");
            }

            
           const string query = @"
            SELECT  * from AppData.FinancialYear
            WHERE (StartYear LIKE @searchTerm OR Id LIKE @searchTerm) and IsDeleted = 0
            ORDER BY ID DESC";
           
          var financialYears = await _dbConnection.QueryAsync<Core.Domain.Entities.FinancialYear>(query, new { SearchTerm = $"%{searchTerm}%" });
            return financialYears.ToList();
        }






    }
}