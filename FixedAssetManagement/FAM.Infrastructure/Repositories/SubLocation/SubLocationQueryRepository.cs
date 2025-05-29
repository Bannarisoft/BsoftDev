using System.Data;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.ISubLocation;
using Dapper;

namespace FAM.Infrastructure.Repositories.SubLocation
{
    public class SubLocationQueryRepository : ISubLocationQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IIPAddressService _ipAddressService;

        public SubLocationQueryRepository(IDbConnection dbConnection, IIPAddressService ipAddressService)
        {
            _dbConnection = dbConnection;
            _ipAddressService = ipAddressService;
        }
        public async Task<(List<Core.Domain.Entities.SubLocation>, int)> GetAllSubLocationAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var UnitId = _ipAddressService.GetUnitId();

            var query = $$"""
             DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
               FROM FixedAsset.SubLocation 
              WHERE IsDeleted = 0 AND UnitId=@UnitId
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
            IsDeleted = 0 AND UnitId=@UnitId
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (Code LIKE @Search OR SubLocationName LIKE @Search )")}}
                ORDER BY Id DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                SELECT @TotalCount AS TotalCount;
            """;


            var parameters = new
            {
                Search = $"%{SearchTerm}%",
                Offset = (PageNumber - 1) * PageSize,
                PageSize,
                UnitId
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

        public async Task<Core.Domain.Entities.SubLocation?> GetBySubLocationNameAsync(string name, int? id = null)
        {
            var query = """
                 SELECT * FROM FixedAsset.SubLocation
                 WHERE SubLocationName = @SubLocationName AND IsDeleted = 0
                 """;

            var parameters = new DynamicParameters(new { SubLocationName = name });

            if (id is not null)
            {
                query += " AND Id != @Id";
                parameters.Add("Id", id);
            }

            return await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.SubLocation>(query, parameters);
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