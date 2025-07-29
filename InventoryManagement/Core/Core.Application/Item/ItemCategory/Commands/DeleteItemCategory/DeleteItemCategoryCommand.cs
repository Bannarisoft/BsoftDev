using MediatR;

namespace Core.Application.Item.ItemCategory.Commands.DeleteItemCategory
{
    public class DeleteItemCategoryCommand : IRequest<int> 
    {
        public int Id { get; set; }
    }
}