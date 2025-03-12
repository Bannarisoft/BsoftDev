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
         public async Task<(List<Division>,int)> GetAllDivisionAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
                 var query = $$"""
             DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
               FROM AppData.Division 
              WHERE IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (ShortName LIKE @Search OR Name LIKE @Search)")}};

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

                SELECT @TotalCount AS TotalCount;
            """;

            
             var parameters = new
                       {
                           Search = $"%{SearchTerm}%",
                           Offset = (PageNumber - 1) * PageSize,
                           PageSize
                       };

               var division = await _dbConnection.QueryMultipleAsync(query, parameters);
             var divisionlist = (await division.ReadAsync<Division>()).ToList();
             int totalCount = (await division.ReadFirstAsync<int>());
            return (divisionlist, totalCount);
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
      
        public async Task<List<Division>>  GetDivision(string searchPattern, List<UserCompany> userCompanies)
        {
            var CompanyId = new List<int>();
            foreach (var userCompany in userCompanies)
            {
                CompanyId.Add(userCompany.CompanyId);
            }
            if (CompanyId is null || !CompanyId.Any())
                return new List<Division>();

            var query = $@"
        SELECT Id, Name 
        FROM AppData.Division 
        WHERE IsDeleted = 0 
        AND Name LIKE @SearchPattern
        AND CompanyId IN ({string.Join(",", CompanyId)})";
                
            
            var parameters = new 
              { 
                  SearchPattern = $"%{searchPattern ?? string.Empty}%"
              };

            var divisions = await _dbConnection.QueryAsync<Division>(query, parameters);
            return divisions.ToList();
        }
           public async Task<bool>SoftDeleteValidation(int Id)
            {
                                const string query = @"
                           SELECT 1 
                           FROM [AppData].[Unit] 
                           WHERE DivisionId = @Id AND IsDeleted = 0;";
                    
                       using var multi = await _dbConnection.QueryMultipleAsync(query, new { Id = Id });
                    
                       
                       var divisionExists = await multi.ReadFirstOrDefaultAsync<int?>();
                    
                       return divisionExists.HasValue ;
            }
    }
}