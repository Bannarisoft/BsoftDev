using Core.Application.Item.ItemDetail.Queries.GetAllItems;

namespace Core.Application.Common.Interfaces.Item.ItemDetail.Commands
{
    public interface IItemUomCommandRepository
    {
        Task UpdateAsync(int itemId, IEnumerable<ItemUomDto> rows, CancellationToken ct = default);
        Task<List<ItemUomDto>> GetByItemIdAsync(int itemId, CancellationToken ct = default);
    }
}
