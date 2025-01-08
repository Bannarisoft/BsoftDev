using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IDivision;
using Core.Application.Divisions.Queries.GetDivisions;
using Core.Application.Common;
using System.Data;
using Dapper;

namespace BSOFT.Infrastructure.Repositories.Divisions
{
    public class DivisionQueryRepository : IDivisionQueryRepository
    {
        private readonly IDbConnection _dbConnection;        
        public DivisionQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
         public async Task<List<Division>> GetAllDivisionAsync()
        {
            //return await _applicationDbContext.Divisions.ToListAsync();

             const string query = @"
            SELECT 
                Id, 
                ShortName,
                Name,
                CompanyId,
                IsActive
            FROM AppData.Division";
            return (await _dbConnection.QueryAsync<Division>(query)).ToList();
        }      
         public async Task<Division> GetByIdAsync(int id)
        {
            //return await _applicationDbContext.Divisions.AsNoTracking()
                //.FirstOrDefaultAsync(b => b.Id == id);

             const string query = "SELECT * FROM AppData.Division WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Division>(query, new { id });
        }
      
        public async Task<List<Division>>  GetDivision(string searchPattern)
        {
            if (string.IsNullOrWhiteSpace(searchPattern))
            {
                throw new ArgumentException("DivisionName cannot be null or empty.", nameof(searchPattern));
            }

            const string query = @"
                SELECT Id, Name 
                FROM AppData.Division 
                WHERE Name LIKE @SearchPattern";
                
            // Update the object to use SearchPattern instead of Name
            var divisions = await _dbConnection.QueryAsync<Division>(query, new { SearchPattern = $"%{searchPattern}%" });
            return divisions.ToList();
        }
    }
}