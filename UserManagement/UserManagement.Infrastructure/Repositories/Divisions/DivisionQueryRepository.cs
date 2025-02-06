using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Data;
using Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IDivision;
using Core.Application.Divisions.Queries.GetDivisions;
using Core.Application.Common;
using System.Data;
using Dapper;

namespace UserManagement.Infrastructure.Repositories.Divisions
{
    public class DivisionQueryRepository : IDivisionQueryRepository
    {
        private readonly IDbConnection _dbConnection;        
        public DivisionQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
         public async Task<List<Division>> GetAllDivisionAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
                 var query = $$"""
                SELECT 
                Id, 
                ShortName,
                Name,
                CompanyId,
                IsActive
            FROM AppData.Division 
            WHERE 
            IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (ShortName LIKE @Search OR Name LIKE @Search )")}}
                ORDER BY Id desc
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

            
             var parameters = new
                       {
                           Search = $"%{SearchTerm}%",
                           Offset = (PageNumber - 1) * PageSize,
                           PageSize
                       };
            return (await _dbConnection.QueryAsync<Division>(query,parameters)).ToList();
        }   
        public async Task<Division?> GetByDivisionnameAsync(string name, int? id = null)
        {
              var query = """
                 SELECT * FROM AppData.Division 
                 WHERE Name = @Name AND IsDeleted = 0
                 """;

             var parameters = new DynamicParameters(new { Name = name });

             if (id is not null)
             {
                 query += " AND Id != @Id";
                 parameters.Add("Id", id);
             }

            return await _dbConnection.QueryFirstOrDefaultAsync<Division>(query, parameters);
        } 

         public async Task<Division> GetByIdAsync(int id)
        {
            

             const string query = "SELECT * FROM AppData.Division WHERE Id = @Id AND IsDeleted = 0";
            return await _dbConnection.QueryFirstOrDefaultAsync<Division>(query, new { id });
        }
      
        public async Task<List<Division>>  GetDivision(string searchPattern)
        {
            

            const string query = @"
                SELECT Id, Name 
                FROM AppData.Division 
                WHERE IsDeleted = 0 AND Name LIKE @SearchPattern";
                
            
            var divisions = await _dbConnection.QueryAsync<Division>(query, new { SearchPattern = $"%{searchPattern}%" });
            return divisions.ToList();
        }
    }
}