using System.Data;
using Core.Application.Common.Interfaces.Item.ItemCategory;
using Core.Application.Item.ItemCategory.Queries.GetItemCategory;
using Core.Application.Item.ItemCategory.Queries.GetItemCategoryAutoComplete;
using Dapper;

namespace  InventoryManagement.Infrastructure.Repositories.Item.ItemCategory
{
    public class ItemCategoryQueryRepository : IItemCategoryQueryRepository
    {
        private readonly IDbConnection _dbConnection;       

        public ItemCategoryQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;            
        }       
        public async Task<ItemCategoryDto> GetByIdAsync(int Id)
        {
            const string query = @"
        WITH CategoryTree AS (
            SELECT IC.Id,IC.ItemCategoryName,IG.Id AS ItemGroupId,IG.ItemGroupName,IC.IsGroup,IC.ParentCategoryId,IC1.ItemCategoryName AS ParentCategoryName,
                IC.IsBudgetApplicable,IC.IsActive,IC.IsDeleted,IC.CreatedBy,IC.CreatedDate,IC.CreatedByName,IC.CreatedIP,IC.ModifiedBy,
                IC.ModifiedDate,IC.ModifiedByName,IC.ModifiedIP
            FROM Inventory.ItemCategory IC
            INNER JOIN Inventory.ItemGroup IG ON IG.Id = IC.ItemGroupId
            LEFT JOIN Inventory.ItemCategory IC1 ON IC.ParentCategoryId = IC1.Id -- ✅ FIXED
            WHERE IC.Id = @Id AND IC.IsDeleted = 0
            UNION ALL
            SELECT IC.Id,IC.ItemCategoryName,IG.Id AS ItemGroupId,IG.ItemGroupName,IC.IsGroup,IC.ParentCategoryId,CT.ItemCategoryName AS ParentCategoryName,
                IC.IsBudgetApplicable,IC.IsActive,IC.IsDeleted,IC.CreatedBy,IC.CreatedDate,IC.CreatedByName,IC.CreatedIP,IC.ModifiedBy,
                IC.ModifiedDate,IC.ModifiedByName,IC.ModifiedIP
            FROM Inventory.ItemCategory IC
            INNER JOIN Inventory.ItemGroup IG ON IG.Id = IC.ItemGroupId
            INNER JOIN CategoryTree CT ON IC.ParentCategoryId = CT.Id
            WHERE IC.IsDeleted = 0)
			  SELECT * FROM CategoryTree;
            ";

            var allCategories = (await _dbConnection.QueryAsync<ItemCategoryDto>(query, new { Id })).ToList();

            // Create lookup dictionary
            var lookup = allCategories.ToDictionary(x => x.Id);

            // Build nested subGroups
            foreach (var node in allCategories)
            {
                if (node.ParentCategoryId.HasValue && lookup.ContainsKey(node.ParentCategoryId.Value))
                {
                    lookup[node.ParentCategoryId.Value].SubGroups.Add(node);
                }
            }

            // Return the requested node with its tree
            return lookup.TryGetValue(Id, out var root) ? root : null;
        }
        public async Task<(IEnumerable<dynamic>, int)> GetAllItemCategoryAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var query = $$"""
                DECLARE @TotalCount INT;
                SELECT @TotalCount = COUNT(*) 
                FROM Inventory.ItemCategory 
                WHERE IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (ItemCategoryName LIKE @Search )")}};

                SELECT 
                    IC.Id, IC.ItemCategoryName, IG.Id AS ItemGroupId, IG.ItemGroupName,
                    IC.IsGroup, IC.ParentCategoryId,
                    IC1.ItemCategoryName AS ParentCategoryName,
                    IC.IsBudgetApplicable, IC.IsActive, IC.IsDeleted,
                    IC.CreatedBy, IC.CreatedDate, IC.CreatedByName, IC.CreatedIP,
                    IC.ModifiedBy, IC.ModifiedDate, IC.ModifiedByName, IC.ModifiedIP
                FROM Inventory.ItemCategory IC
                INNER JOIN Inventory.ItemGroup IG ON IG.Id = IC.ItemGroupId
                LEFT JOIN Inventory.ItemCategory IC1 ON IC.ParentCategoryId = IC1.Id
                WHERE IC.IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (IC.ItemCategoryName LIKE @Search )")}}

                SELECT @TotalCount AS TotalCount;
                """;

                var parameters = new
                {
                    Search = $"%{SearchTerm}%",
                    Offset = (PageNumber - 1) * PageSize,
                    PageSize = PageSize
                };

                var result = await _dbConnection.QueryMultipleAsync(query, parameters);

                var flatList = (await result.ReadAsync<ItemCategoryDto>()).ToList();
                int totalCount = await result.ReadFirstAsync<int>();

                // Build hierarchy
                var lookup = flatList.ToDictionary(x => x.Id);
                foreach (var node in flatList)
                {
                    if (node.ParentCategoryId.HasValue && lookup.ContainsKey(node.ParentCategoryId.Value))
                    {
                        var parent = lookup[node.ParentCategoryId.Value];
                        parent.SubGroups.Add(node);
                    }
                }

                // Return only root-level nodes (where ParentCategoryId == null)
                var rootCategories = flatList
                    .Where(x => x.ParentCategoryId == null)
                    .ToList();

                return (rootCategories, totalCount);
        }
        public async Task<bool> SoftDeleteValidation(int Id)
        {
            const string query = @"
                    SELECT 1 
                    FROM Inventory.ItemCategory 
                    WHERE ParentCategoryId = @Id AND IsDeleted = 0;                 
                    ";
            using var multi = await _dbConnection.QueryMultipleAsync(query, new { Id = Id });
            var notificationConfigExists = await multi.ReadFirstOrDefaultAsync<int?>();
            return notificationConfigExists.HasValue;
        }
        public async Task<bool> NotFoundAsync(int Id)
        {
            var query = "SELECT COUNT(1) FROM Inventory.ItemCategory WHERE Id = @Id AND IsDeleted = 0";             
            var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = Id });
            return count > 0;
        }
        public async Task<List<ItemCategoryAutoCompleteDto>> GetItemCategoryAutoCompleteAsync(string searchPattern)
        {
            searchPattern = searchPattern ?? string.Empty;
            const string query = @"
             SELECT IC.Id, IC.ItemCategoryName,IC1.ItemCategoryName AS ParentCategoryName
            FROM Inventory.ItemCategory IC       
            LEFT JOIN Inventory.ItemCategory IC1 ON IC.ParentCategoryId = IC1.Id     
            WHERE IC.IsDeleted = 0 
            AND IC.ItemCategoryName LIKE @SearchPattern";
            var parameters = new
            {
                SearchPattern = $"%{searchPattern}%"
            };
            var notificationConfig = await _dbConnection.QueryAsync<ItemCategoryAutoCompleteDto>(query, parameters);
            return notificationConfig.ToList();
        }
    }
}