using System.Data;
using Core.Application.Common.Interfaces.ILocation;
using Core.Domain.Entities;
using Dapper;

namespace FAM.Infrastructure.Repositories.Locations
{
    public class LocationQueryRepository : ILocationQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        public LocationQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
            
        }        
        public async Task<(List<Location>, int)> GetAllLocationAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var query = $$"""
             DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
               FROM FixedAsset.Location 
              WHERE IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (Code LIKE @Search OR LocationName LIKE @Search)")}};

                SELECT 
                Id, 
                Code,
                LocationName,
                Description,
                SortOrder,
                UnitId,
                DepartmentId
                IsActive
            FROM FixedAsset.Location 
            WHERE 
            IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (Code LIKE @Search OR LocationName LIKE @Search )")}}
                ORDER BY Id DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                SELECT @TotalCount AS TotalCount;
            """;

            
             var parameters = new
                       {
                           Search = $"%{SearchTerm}%",
                           Offset = (PageNumber - 1) * PageSize,
                           PageSize
                       };

               var location = await _dbConnection.QueryMultipleAsync(query, parameters);
             var locationlist = (await location.ReadAsync<Location>()).ToList();
             int totalCount = (await location.ReadFirstAsync<int>());
            return (locationlist, totalCount);
        }

        public async Task<Location> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM FixedAsset.Location WHERE Id = @Id AND IsDeleted = 0";
            return await _dbConnection.QueryFirstOrDefaultAsync<Location>(query, new { id });
        }

        public async Task<Location?> GetByLocationNameAsync(string name, int? id = null)
        {
            var query = """
                 SELECT * FROM FixedAsset.Location
                 WHERE LocationName = @LocationName AND IsDeleted = 0
                 """;

             var parameters = new DynamicParameters(new { LocationName = name });

             if (id is not null)
             {
                 query += " AND Id != @Id";
                 parameters.Add("Id", id);
             }

            return await _dbConnection.QueryFirstOrDefaultAsync<Location>(query, parameters);
        }
        public async Task<List<Location>> GetLocation(string searchPattern=null)
        {
            const string query = @"
                SELECT Id, LocationName 
                FROM FixedAsset.Location 
                WHERE IsDeleted = 0 AND LocationName LIKE @SearchPattern";
                
            var locations = await _dbConnection.QueryAsync<Location>(query, new { SearchPattern = $"%{searchPattern}%" });
            return locations.ToList();
        }
    }
}