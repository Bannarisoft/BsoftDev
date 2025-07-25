using System.Data;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.ISubLocation;
using Dapper;

namespace FAM.Infrastructure.Repositories.SubLocation
{
    public class SubLocationQueryRepository : ISubLocationQueryRepository
    {
        private readonly IDbConnection _dbConnection;

        public SubLocationQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<(List<Core.Domain.Entities.SubLocation>, int)> GetAllSubLocationAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var query = $$"""
             DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
               FROM FixedAsset.SubLocation 
              WHERE IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (Code LIKE @Search OR SubLocationName LIKE @Search)")}};

            SELECT 
                Id, 
                Code,
                SubLocationName,
                Description,
                LocationId,
                UnitId,
                DepartmentId,
                IsActive,
                CreatedDate,
                CreatedByName
            FROM FixedAsset.SubLocation 
            WHERE 
            IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (Code LIKE @Search OR SubLocationName LIKE @Search )")}}
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

            var sublocation = await _dbConnection.QueryMultipleAsync(query, parameters);
            var sublocationlist = (await sublocation.ReadAsync<Core.Domain.Entities.SubLocation>()).ToList();
            int totalCount = (await sublocation.ReadFirstAsync<int>());
            return (sublocationlist, totalCount);
        }

        public async Task<Core.Domain.Entities.SubLocation> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM FixedAsset.SubLocation WHERE Id = @Id AND IsDeleted = 0";
            return await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.SubLocation>(query, new { id });
        }

        public async Task<Core.Domain.Entities.SubLocation?> GetBySubLocationNameAsync(string name, int DepartmentId,int LocationId,int UnitId, int? id = null)
        {
            var query = @"
            SELECT 
                S.Id,
                S.Code,
                S.SubLocationName,
                S.Description,
                S.UnitId,
                S.DepartmentId,
				S.LocationId,
                S.IsActive,
                S.IsDeleted,
                S.CreatedBy,
                S.CreatedDate,
                S.CreatedByName,
                S.CreatedIP,
                S.ModifiedBy,
                S.ModifiedDate,
                S.ModifiedByName,
                S.ModifiedIP
            FROM FixedAsset.SubLocation S
            JOIN Bannari.AppData.Department D ON D.Id = S.DepartmentId
            JOIN Bannari.AppData.Unit U ON U.Id = S.UnitId
			JOIN FixedAsset.Location L on L.Id= S.LocationId
            WHERE S.SubLocationName = @SubLocationName AND S.IsDeleted = 0 AND S.DepartmentId = @DepartmentId AND S.LocationId=@LocationId AND S.UnitId = @UnitId
            
        ";

            return await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.SubLocation>(query, new { SubLocationName = name, DepartmentId = DepartmentId , LocationId = LocationId, UnitId = UnitId });
            // var query = """
            //      SELECT * FROM FixedAsset.SubLocation
            //      WHERE SubLocationName = @SubLocationName AND IsDeleted = 0
            //      """;

            // var parameters = new DynamicParameters(new { SubLocationName = name });

            // if (id is not null)
            // {
            //     query += " AND Id != @Id";
            //     parameters.Add("Id", id);
            // }

            // return await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.SubLocation>(query, parameters);
        }

        public async Task<List<Core.Domain.Entities.SubLocation>> GetSubLocation(string searchPattern)
        {
            const string query = @"
                SELECT Id, SubLocationName 
                FROM FixedAsset.SubLocation 
                WHERE IsDeleted = 0 AND SubLocationName LIKE @SearchPattern";

            var locations = await _dbConnection.QueryAsync<Core.Domain.Entities.SubLocation>(query, new { SearchPattern = $"%{searchPattern}%" });
            return locations.ToList();
        }
    }
}