using Core.Application.Item.ItemCategory.Queries.GetItemCategory;
using MediatR;

namespace Core.Application.Item.ItemCategory.Queries.GetItemCategoryById
{
    public class GetItemCategoryByIdQuery : IRequest<ItemCategoryDto>
    {
        public int Id { get; set; }
    }
}