namespace Core.Application.Common.Interfaces.Item.ItemCategory
{
    public interface IItemCategoryCommandRepository
    {
        Task<int> CreateAsync(Domain.Entities.Item.ItemCategory itemCategory);
        Task<int> UpdateAsync(int assetId, Domain.Entities.Item.ItemCategory itemCategory);
        Task<int> DeleteAsync(int assetId, Domain.Entities.Item.ItemCategory itemCategory);
        Task<bool> ExistsByNameAsync(string? name, int itemGroupId);
        Task<bool> IsNameDuplicateAsync(string? name, int itemGroupId, int excludeId);               
    }
}