using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetCategories.Queries.GetAssetCategories;
using Core.Application.Common.Interfaces.IAssetCategories;
using Dapper;

namespace FAM.Infrastructure.Repositories.AssetCategories
{
    public class AssetCategoriesQueryRepository : IAssetCategoriesQueryRepository
    {
        private readonly IDbConnection _dbConnection; 

        public AssetCategoriesQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<(List<Core.Domain.Entities.AssetCategories>, int)> GetAllAssetCategoriesAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
             var query = $$"""
             DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
               FROM FixedAsset.AssetCategories
              WHERE IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CategoryName LIKE @Search OR Code LIKE @Search)")}};

                SELECT 
                Id, 
                Code,
                CategoryName,
                Description,
                AssetGroupId,
                SortOrder,
                IsActive
            FROM FixedAsset.AssetCategories 
            WHERE 
            IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CategoryName LIKE @Search OR Code LIKE @Search )")}}
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

             var assetCategories = await _dbConnection.QueryMultipleAsync(query, parameters);
             var assetcategoreplist = (await assetCategories.ReadAsync<Core.Domain.Entities.AssetCategories>()).ToList();
             int totalCount = (await assetCategories.ReadFirstAsync<int>());
             return (assetcategoreplist, totalCount);
        }

        public async Task<List<Core.Domain.Entities.AssetCategories>> GetAssetCategories(string searchPattern)
        {
            searchPattern = searchPattern ?? string.Empty; // Prevent null issues

            const string query = @"
             SELECT Id, CategoryName 
            FROM FixedAsset.AssetCategories 
            WHERE IsDeleted = 0 
            AND CategoryName LIKE @SearchPattern";  
            var parameters = new 
            { 
            SearchPattern = $"%{searchPattern}%" 
            };

            var assetCategories = await _dbConnection.QueryAsync<Core.Domain.Entities.AssetCategories>(query, parameters);
            return assetCategories.ToList();
        }

        public async Task<List<AssetCategoriesAutoCompleteDto?>> GetByAssetgroupIdAsync(int AssetGroupId)
        {
            const string query = @"
            SELECT 
                    b.Id,
                    b.CategoryName 
                    from 
                    FixedAsset.AssetGroup a INNER JOIN FixedAsset.AssetCategories b 
                    on a.Id=b.AssetGroupId 
                    and  b.IsDeleted=0 
                    and a.IsDeleted=0 and a.Id=@AssetGroupId ";

            var assetCategories = await _dbConnection.QueryAsync<AssetCategoriesAutoCompleteDto>(query, new { AssetGroupId });

            return assetCategories.ToList(); // Ensure it returns a List
        }

    

        public async Task<Core.Domain.Entities.AssetCategories?> GetByIdAsync(int Id)
        {
            const string query = @"
                    SELECT * 
                    FROM FixedAsset.AssetCategories 
                    WHERE Id = @Id AND IsDeleted = 0";
                    var assetCategories = await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.AssetCategories>(query, new { Id });
                    return assetCategories;
        }

    
    }
}