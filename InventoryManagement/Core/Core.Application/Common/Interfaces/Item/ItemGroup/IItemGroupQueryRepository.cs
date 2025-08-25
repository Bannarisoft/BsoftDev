using Core.Application.Item.ItemGroup.Queries.GetItemGroup;
using Core.Application.Item.ItemGroup.Queries.GetItemGroupAutoComplete;

namespace Core.Application.Common.Interfaces.Item.ItemGroup
{
    public interface IItemGroupQueryRepository
    {
        Task<ItemGroupDto> GetByIdAsync(int id);
        Task<(IEnumerable<dynamic>, int)> GetAllItemGroupAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<List<ItemGroupAutoCompleteDto>> GetItemGroupAutoCompleteAsync(string searchPattern);
        Task<bool> SoftDeleteValidation(int Id);
        Task<List<Core.Domain.Entities.Item.ItemGroup>> GetAllItemGroupsAsync();       
            
    }
}