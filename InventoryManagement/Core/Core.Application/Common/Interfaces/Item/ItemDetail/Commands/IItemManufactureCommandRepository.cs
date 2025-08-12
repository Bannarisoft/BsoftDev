using Core.Application.Item.ItemDetail.Queries.GetAllItems;

namespace Core.Application.Common.Interfaces.Item.ItemDetail.Commands
{
    public interface IItemManufactureCommandRepository
    {
        Task UpdateAsync(int itemId, IEnumerable<ItemManufactureDto> rows, CancellationToken ct = default);
        Task<List<ItemManufactureDto>> GetByItemIdAsync(int itemId, CancellationToken ct = default);
    }
}