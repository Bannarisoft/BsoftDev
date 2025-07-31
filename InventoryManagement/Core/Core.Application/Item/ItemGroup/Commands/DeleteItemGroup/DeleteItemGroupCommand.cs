using MediatR;

namespace Core.Application.Item.ItemGroup.Commands.DeleteItemGroup
{
    public class DeleteItemGroupCommand : IRequest<int> 
    {
        public int Id { get; set; }
    }
}