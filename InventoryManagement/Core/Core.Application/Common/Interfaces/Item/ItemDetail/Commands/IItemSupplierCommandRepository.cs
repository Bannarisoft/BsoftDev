// IItemSupplierCommandRepository.cs
using Core.Application.Item.ItemDetail.Queries.GetAllItems;

public interface IItemSupplierCommandRepository
{
    Task<IReadOnlyList<Core.Domain.Entities.Item.ItemDetail.ItemSupplier>> GetByItemIdAsync(int itemId, CancellationToken ct);
    Task UpdateAsync(int itemId, IReadOnlyCollection<ItemSupplierDto> rows, CancellationToken ct);
}
