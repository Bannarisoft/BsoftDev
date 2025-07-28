using Core.Application.Item.ItemCategory.Queries.GetItemCategoryAutoComplete;
using MediatR;

namespace Core.Application.Item.ItemCategory.Queries.GetItemCategoryAutoComplete
{
    public class GetItemCategoryAutoCompleteQuery : IRequest<List<ItemCategoryAutoCompleteDto>>    
    {
        public string? SearchPattern { get; set; }       
    }
}