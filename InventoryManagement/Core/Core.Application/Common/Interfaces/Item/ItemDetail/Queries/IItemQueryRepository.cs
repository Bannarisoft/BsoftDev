using Core.Application.Item.ItemDetail.Queries.GetAllItems;

namespace Core.Application.Common.Interfaces.Item.ItemDetail.Queries
{
    public interface IItemQueryRepository
    {

        Task<(List<ItemListDto> Items, int TotalCount)> GetAllAsync(int page, int size, string? search, bool onlyActive, CancellationToken ct = default);
        Task<ItemDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<string> GetBaseDirectoryAsync(CancellationToken ct = default);
        Task<bool> RemoveImageReferenceAsync(string imagePath);
        Task<string?> GetLatestItemCode(int itemGroupId, int itemCategoryId, CancellationToken ct = default);
         Task<List<string>> GetCandidateItemNamesAsync(string normalizedPrefix, int take = 200, CancellationToken ct = default);
    }
}