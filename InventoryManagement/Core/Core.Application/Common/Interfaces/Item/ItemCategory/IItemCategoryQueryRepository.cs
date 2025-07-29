using Core.Application.Item.ItemCategory.Queries.GetItemCategory;
using Core.Application.Item.ItemCategory.Queries.GetItemCategoryAutoComplete;

namespace Core.Application.Common.Interfaces.Item.ItemCategory
{
    public interface IItemCategoryQueryRepository
    {
        Task<ItemCategoryDto> GetByIdAsync(int id);
        Task<(IEnumerable<dynamic>, int)> GetAllItemCategoryAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<List<ItemCategoryAutoCompleteDto>> GetItemCategoryAutoCompleteAsync(string searchPattern);
        Task<bool> SoftDeleteValidation(int Id);                
    }
}