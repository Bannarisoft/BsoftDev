using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetSubCategories.Queries.GetAssetSubCategories;
using Core.Application.Common.Interfaces.IAssetSubCategories;
using Dapper;

namespace FAM.Infrastructure.Repositories.AssetSubCategories
{
    public class AssetSubCategoriesQueryRepository : IAssetSubCategoriesQueryRepository
    {
        private readonly IDbConnection _dbConnection; 
          public AssetSubCategoriesQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<(List<Core.Domain.Entities.AssetSubCategories>, int)> GetAllAssetSubCategoriesAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
              var query = $$"""
             DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
               FROM FixedAsset.AssetSubCategories
              WHERE IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (SubCategoryName LIKE @Search OR Code LIKE @Search)")}};

                SELECT 
                Id, 
                Code,
                SubCategoryName,
                Description,
                AssetCategoriesId,
                SortOrder,
                IsActive
            FROM FixedAsset.AssetSubCategories 
            WHERE 
            IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (SubCategoryName LIKE @Search OR Code LIKE @Search )")}}
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

             var assetsubCategories = await _dbConnection.QueryMultipleAsync(query, parameters);
             var assetsubcategoreplist = (await assetsubCategories.ReadAsync<Core.Domain.Entities.AssetSubCategories>()).ToList();
             int totalCount = (await assetsubCategories.ReadFirstAsync<int>());
             return (assetsubcategoreplist, totalCount);
        }

        public async Task<List<Core.Domain.Entities.AssetSubCategories>> GetAssetSubCategories(string searchPattern)
        {
             searchPattern = searchPattern ?? string.Empty; // Prevent null issues

            const string query = @"
             SELECT Id, SubCategoryName 
            FROM FixedAsset.AssetSubCategories 
            WHERE IsDeleted = 0 
            AND SubCategoryName LIKE @SearchPattern";  
            var parameters = new 
            { 
            SearchPattern = $"%{searchPattern}%" 
            };

            var assetSubCategories = await _dbConnection.QueryAsync<Core.Domain.Entities.AssetSubCategories>(query, parameters);
            return assetSubCategories.ToList();
        }

        public async Task<Core.Domain.Entities.AssetSubCategories?> GetByIdAsync(int Id)
        {
             const string query = @"
                    SELECT * 
                    FROM FixedAsset.AssetSubCategories 
                    WHERE Id = @Id AND IsDeleted = 0";
                    var assetSubCategories = await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.AssetSubCategories>(query, new { Id });
                    return assetSubCategories;
        }

        public async Task<List<AssetSubCategoriesAutoCompleteDto?>> GetSubcategoriesByAssetCategoryIdAsync(int AssetCategoriesId)
        {
             const string query = @"
                    SELECT 
                    b.Id,
                    b.SubCategoryName 
                    from 
                    FixedAsset.AssetCategories a INNER JOIN FixedAsset.AssetSubCategories b 
                    on a.Id=b.AssetCategoriesId 
                    and b.IsDeleted=0 
                    and a.IsDeleted=0 
                    and a.Id=@AssetCategoriesId ";

            var assetsubCategories = await _dbConnection.QueryAsync<AssetSubCategoriesAutoCompleteDto>(query, new { AssetCategoriesId });

            return assetsubCategories.ToList(); // Ensure it returns a List
        }
    }
}