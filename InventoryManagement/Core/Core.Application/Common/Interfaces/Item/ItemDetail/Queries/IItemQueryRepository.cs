using Core.Application.Item.ItemDetail.Queries.GetAllItems;

namespace Core.Application.Common.Interfaces.Item.ItemDetail.Queries
{
    public interface IItemQueryRepository
    {
        /* Task<(List<ItemDto> Items, int TotalCount)> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? search,bool onlyActive, 
            CancellationToken ct = default);
        Task<ItemDto?> GetByIdAsync(int id, CancellationToken ct = default); */

        Task<(List<ItemListDto> Items, int TotalCount)> GetAllAsync(int page, int size, string? search, bool onlyActive, CancellationToken ct = default);
        Task<ItemDto?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}