using Core.Application.Item.ItemDetail.Queries.GetAllItems;

namespace Core.Application.Common.Interfaces.Item.ItemDetail.Commands
{
    public interface IItemManufactureCommandRepository
    {
        Task<IReadOnlyList<Core.Domain.Entities.Item.ItemDetail.ItemManufacture>> GetByItemIdAsync(int itemId, CancellationToken ct);
        Task UpdateAsync(int itemId, IReadOnlyCollection<ItemManufactureDto> rows, CancellationToken ct);
    }
}