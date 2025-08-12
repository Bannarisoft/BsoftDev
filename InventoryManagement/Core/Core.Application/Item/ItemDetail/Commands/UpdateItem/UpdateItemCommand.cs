
using Core.Application.Item.ItemDetail.Queries.GetAllItems;
using MediatR;

namespace Core.Application.Item.ItemDetail.Commands.UpdateItem
{
    public sealed class UpdateItemCommand :  IRequest<Unit>
    {
        public int Id { get; init; }                        
        public ItemDto Payload { get; init; } = default!;
    }
}
