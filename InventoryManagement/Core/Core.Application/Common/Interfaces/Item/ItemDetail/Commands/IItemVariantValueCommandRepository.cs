using Core.Application.Item.ItemDetail.Queries.GetAllItems;

namespace Core.Application.Common.Interfaces.Item.ItemDetail.Commands
{
    public interface IItemVariantValueCommandRepository
    {
        Task UpsertListAsync(int itemId, IEnumerable<VariantValueDto> values, CancellationToken ct = default);
        
        Task AddAsync(int itemId, int attributeId, string optionValue, CancellationToken ct = default);

        Task SetSingleAsync(int itemId, int attributeId, string optionValue, CancellationToken ct = default);

        Task RemoveAsync(int itemId, int attributeId, string optionValue, CancellationToken ct = default);

        Task ClearForItemAsync(int itemId, CancellationToken ct = default);
    }
}