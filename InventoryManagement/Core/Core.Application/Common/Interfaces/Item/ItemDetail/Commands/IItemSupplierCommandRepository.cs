using Core.Application.Item.ItemDetail.Queries.GetAllItems;

namespace Core.Application.Common.Interfaces.Item.ItemDetail.Commands
{
    public interface IItemSupplierCommandRepository
    {
        Task UpdateAsync(int itemId, IEnumerable<ItemSupplierDto> suppliers, CancellationToken ct = default);
        Task<List<ItemSupplierDto>> GetByItemIdAsync(int itemId, CancellationToken ct = default);
    }
}