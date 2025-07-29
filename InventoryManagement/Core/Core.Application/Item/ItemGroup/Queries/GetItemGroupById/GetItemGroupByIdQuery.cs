using Core.Application.Item.ItemGroup.Queries.GetItemGroup;
using MediatR;

namespace Core.Application.Item.ItemGroup.Queries.GetItemGroupById
{
    public class GetItemGroupByIdQuery : IRequest<ItemGroupDto>
    {
        public int Id { get; set; }
    }
}