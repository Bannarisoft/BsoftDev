using Core.Application.Item.ItemDetail.Queries.GetAllItems;
using MediatR;

namespace Core.Application.Item.ItemDetail.Queries.GetItemById
{
    public sealed class GetItemByIdQuery : IRequest<ItemDto?>
    {
        public int Id { get; init; }
    }
}
