using System.Data;
using Core.Application.Common.Interfaces.Item.ItemGroup;
using Core.Application.Item.ItemGroup.Queries.GetItemGroup;
using Core.Application.Item.ItemGroup.Queries.GetItemGroupAutoComplete;
using Dapper;

namespace  InventoryManagement.Infrastructure.Repositories.Item.ItemGroup
{
    public class ItemGroupQueryRepository : IItemGroupQueryRepository
    {
        private readonly IDbConnection _dbConnection;       

        public ItemGroupQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;            
        }       
        public async Task<ItemGroupDto> GetByIdAsync(int Id)
        {
            const string query = @" select 
                    IG.Id,ItemGroupCode, ItemGroupName,IG,UnitId ,
                    ,IC.IsActive, IC.IsDeleted, IC.CreatedBy, IC.CreatedDate, IC.CreatedByName, IC.CreatedIP, IC.ModifiedBy, IC.ModifiedDate, IC.ModifiedByName, IC.ModifiedIP
                    FROM  Inventory.ItemGroup IG
                    WHERE IG.Id = @Id AND IG.IsDeleted = 0";

            var notificationConfig = await _dbConnection.QueryFirstOrDefaultAsync<ItemGroupDto>(query, new { Id });
            return notificationConfig;
        }
        public async Task<(IEnumerable<dynamic>, int)> GetAllItemGroupAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var query = $$"""
            DECLARE @TotalCount INT;
            SELECT @TotalCount = COUNT(*) 
            FROM Inventory.ItemGroup 
            WHERE IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (ModuleName LIKE @Search)")}};

            SELECT 
                IG.Id,ItemGroupCode, ItemGroupName,IG,UnitId ,
                ,IC.IsActive, IC.IsDeleted, IC.CreatedBy, IC.CreatedDate, IC.CreatedByName, IC.CreatedIP, IC.ModifiedBy, IC.ModifiedDate, IC.ModifiedByName, IC.ModifiedIP
                FROM  Inventory.ItemGroup IG
            WHERE IC.IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (ItemGroupName LIKE @Search )")}}
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
            var notificationConfigList = (await notificationConfig.ReadAsync<ItemGroupDto>()).ToList();
            int totalCount = (await notificationConfig.ReadFirstAsync<int>());
            return (notificationConfigList, totalCount);
        }
        public async Task<bool> SoftDeleteValidation(int Id)
        {
            const string query = @"
                    SELECT 1 
                    FROM Inventory.ItemCategory 
                    WHERE ItemGroupId = @Id AND IsDeleted = 0;                 
                    ";
            using var multi = await _dbConnection.QueryMultipleAsync(query, new { Id = Id });
            var notificationConfigExists = await multi.ReadFirstOrDefaultAsync<int?>();
            return notificationConfigExists.HasValue;
        }
        public async Task<bool> NotFoundAsync(int Id)
        {
            var query = "SELECT COUNT(1) FROM Inventory.ItemGroup WHERE Id = @Id AND IsDeleted = 0";             
            var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = Id });
            return count > 0;
        }
        public async Task<List<ItemGroupAutoCompleteDto>> GetItemGroupAutoCompleteAsync(string searchPattern)
        {
            searchPattern = searchPattern ?? string.Empty;
            const string query = @"
             SELECT IC.Id, IC.ItemGroupName
            FROM Inventory.ItemGroup IC            
            WHERE IC.IsDeleted = 0 
            AND ModuleName LIKE @SearchPattern";
            var parameters = new
            {
                SearchPattern = $"%{searchPattern}%"
            };
            var notificationConfig = await _dbConnection.QueryAsync<ItemGroupAutoCompleteDto>(query, parameters);
            return notificationConfig.ToList();
        }
    }
}