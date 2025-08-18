using Core.Application.Item.ItemDetail.Queries.GetAllItems;

namespace Core.Application.Common.Interfaces.Item.ItemDetail.Commands
{
    public interface IItemUomCommandRepository
    {
        Task<IReadOnlyList<Core.Domain.Entities.Item.ItemDetail.ItemUOM>> GetByItemIdAsync(int itemId, CancellationToken ct);
        Task UpdateAsync(int itemId, IReadOnlyCollection<ItemUomDto> rows, CancellationToken ct);
    }
}
