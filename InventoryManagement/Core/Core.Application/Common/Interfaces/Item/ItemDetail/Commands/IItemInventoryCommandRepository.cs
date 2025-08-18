// Core.Application/Common/Interfaces/Item/ItemDetail/Inventory/IItemInventoryCommandRepository.cs

using Core.Domain.Entities.Item.ItemDetail;

namespace Core.Application.Common.Interfaces.Item.ItemDetail.Commands
{
    public interface IItemInventoryCommandRepository
    {
        Task<ItemInventory?> GetByItemIdAsync(int itemId, CancellationToken ct = default);
        Task CreateAsync(ItemInventory inventory, CancellationToken ct = default);
        Task UpdateAsync(ItemInventory entity, CancellationToken ct = default);
    }
}
