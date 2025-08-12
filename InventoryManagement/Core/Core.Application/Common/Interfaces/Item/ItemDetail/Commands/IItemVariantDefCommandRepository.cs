namespace Core.Application.Common.Interfaces.Item.ItemDetail.Commands
{
    public interface IItemVariantDefCommandRepository
    {
        /// Adds ItemVariantDef and returns its Id.
        Task<int> CreateDefAsync(int itemId, int attributeId, CancellationToken ct = default);
        
        Task AddDefOptionsAsync(int itemVariantDefId, IEnumerable<int> optionIds, CancellationToken ct = default);
         Task ReplaceDefsAsync(int itemId,
            IEnumerable<(int attributeId, IEnumerable<int> optionIds)> defs,
            CancellationToken ct = default);
        Task ClearDefsAsync(int itemId, CancellationToken ct = default);
    }
}
