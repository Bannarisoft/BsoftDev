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
            const string query = @" select 
                     IC.Id, ItemCategoryName,IG,Id ItemGroupId,IG.ItemGroupName,IsGroup,IC.ParentCategoryId,IC1.ItemCategoryName ParentCategoryName ,IsBudgetApplicable
                    ,IC.IsActive, IC.IsDeleted, IC.CreatedBy, IC.CreatedDate, IC.CreatedByName, IC.CreatedIP, IC.ModifiedBy, IC.ModifiedDate, IC.ModifiedByName, IC.ModifiedIP
                    FROM  Inventory.ItemCategory IC
                    INNER JOIN Inventory.ItemGroup IG on IG.Id=N=IC.ItemGroupId
                    LEFT JOIN Inventory.ItemCategory IC1 on IC1.ParentCategoryId=IC.Id                 
                    WHERE IC.Id = @Id AND IC.IsDeleted = 0";

            var notificationConfig = await _dbConnection.QueryFirstOrDefaultAsync<ItemCategoryDto>(query, new { Id });
            return notificationConfig;
        }
        public async Task<(IEnumerable<dynamic>, int)> GetAllItemCategoryAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var query = $$"""
            DECLARE @TotalCount INT;
            SELECT @TotalCount = COUNT(*) 
            FROM Inventory.ItemCategory 
            WHERE IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (ModuleName LIKE @Search)")}};

            SELECT 
            IC.Id, ItemCategoryName,IG,Id ItemGroupId,IG.ItemGroupName,IsGroup,IC.ParentCategoryId,IC1.ItemCategoryName ParentCategoryName ,IsBudgetApplicable
            ,IC.IsActive, IC.IsDeleted, IC.CreatedBy, IC.CreatedDate, IC.CreatedByName, IC.CreatedIP, IC.ModifiedBy, IC.ModifiedDate, IC.ModifiedByName, IC.ModifiedIP
            FROM  Inventory.ItemCategory IC
            INNER JOIN Inventory.ItemGroup IG on IG.Id=N=IC.ItemGroupId
            LEFT JOIN Inventory.ItemCategory IC1 on IC1.ParentCategoryId=IC.Id
            WHERE 
            IC.IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (ItemCategoryName LIKE @Search )")}}
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

            var notificationConfig = await _dbConnection.QueryMultipleAsync(query, parameters);
            var notificationConfigList = (await notificationConfig.ReadAsync<ItemCategoryDto>()).ToList();
            int totalCount = (await notificationConfig.ReadFirstAsync<int>());
            return (notificationConfigList, totalCount);
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
             SELECT IC.Id, IC.ItemCategoryName
            FROM Inventory.ItemCategory IC            
            WHERE IC.IsDeleted = 0 
            AND ModuleName LIKE @SearchPattern";
            var parameters = new
            {
                SearchPattern = $"%{searchPattern}%"
            };
            var notificationConfig = await _dbConnection.QueryAsync<ItemCategoryAutoCompleteDto>(query, parameters);
            return notificationConfig.ToList();
        }
    }
}