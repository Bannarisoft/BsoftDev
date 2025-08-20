using Core.Application.Item.ItemDetail.Queries.GetAllItems;

namespace Core.Application.Common.Interfaces.Item.ItemDetail.Commands
{
    public interface IItemVariantValueCommandRepository
    {
        Task UpsertListAsync(int itemId, IEnumerable<VariantValueDto> values, CancellationToken ct = default);
        //Task AddAsync(int itemId, int attributeId, int variantBasedOn,int attributeGroupId, string optionValue, CancellationToken ct = default);
        //Task SetSingleAsync(int itemId, int attributeId, int variantBasedOn,int attributeGroupId, string optionValue, CancellationToken ct = default);
        //Task RemoveAsync(int itemId, int attributeId, int variantBasedOn,int attributeGroupId, string optionValue, CancellationToken ct = default);
        //Task ClearForItemAsync(int itemId, CancellationToken ct = default);
        Task MapOptionToChildAsync(int templateItemId, int attributeId, string optionValue, int childItemId, CancellationToken ct = default); 
        Task AddMissingTemplateOptionsAsync(int templateItemId, IEnumerable<VariantValueDto> options, CancellationToken ct);

    }
}