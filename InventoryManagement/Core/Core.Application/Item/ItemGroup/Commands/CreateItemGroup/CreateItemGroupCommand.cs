
using MediatR;

namespace Core.Application.Item.ItemGroup.Commands.CreateItemGroup
{
    public class CreateItemGroupCommand : IRequest<int>
    {        
        public string? ItemGroupCode { get; set; }
        public string? ItemGroupName { get; set; }  
    }
}