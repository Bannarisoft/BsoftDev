using System.Data;
using Core.Application.Common.Interfaces.IAssetSubGroup;
using Dapper;

namespace FAM.Infrastructure.Repositories.AssetSubGroup
{
    public class AssetSubGroupQueryRepository : IAssetSubGroupQueryRepository
    {
        private readonly IDbConnection _dbConnection; 

        public AssetSubGroupQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Core.Domain.Entities.AssetSubGroup?> GetByIdAsync(int id)
        {
                const string query = @"
                    SELECT * 
                    FROM FixedAsset.AssetSubGroup 
                    WHERE Id = @Id AND IsDeleted = 0";

                    var assetSubGroup = await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.AssetSubGroup>(query, new { id });
                    return assetSubGroup;
        }

         public async Task<(List<Core.Domain.Entities.AssetSubGroup>,int)> GetAllAssetSubGroupAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var query = $$"""
            DECLARE @TotalCount INT;
            SELECT @TotalCount = COUNT(*) 
               FROM FixedAsset.AssetSubGroup
              WHERE IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (SubGroupName LIKE @Search OR Code LIKE @Search)")}};

                SELECT 
                Id, 
                Code,
                SubGroupName,
                SortOrder,
                GroupId,AdditionalDepreciation,SubGroupPercentage,
                IsActive,
                CreatedDate,
                CreatedByName
            FROM FixedAsset.AssetSubGroup 
            WHERE 
            IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (SubGroupName LIKE @Search OR Code LIKE @Search )")}}
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

             var assetSubGroup = await _dbConnection.QueryMultipleAsync(query, parameters);
             var assetSubGroupList = (await assetSubGroup.ReadAsync<Core.Domain.Entities.AssetSubGroup>()).ToList();
             int totalCount = (await assetSubGroup.ReadFirstAsync<int>());
             return (assetSubGroupList, totalCount);
        }

        public async Task<List<Core.Domain.Entities.AssetSubGroup>> GetAssetSubGroups(string searchPattern)
        {
            searchPattern = searchPattern ?? string.Empty; // Prevent null issues

            const string query = @"
             SELECT Id, SubGroupName ,GroupId
            FROM FixedAsset.AssetSubGroup 
            WHERE IsDeleted = 0 
            AND SubGroupName LIKE @SearchPattern";  
            var parameters = new 
            { 
            SearchPattern = $"%{searchPattern}%" 
            };
            var assetSubGroups = await _dbConnection.QueryAsync<Core.Domain.Entities.AssetSubGroup>(query, parameters);
            return assetSubGroups.ToList();
        }
    }
}