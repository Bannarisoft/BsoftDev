
using MediatR;

namespace Core.Application.Item.ItemGroup.Commands.CreateItemGroup
{
    public class CreateItemGroupCommand : IRequest<int>
    {
        public int Id { get; set; }
        public string? ItemGroupCode { get; set; }
        public string? ItemGroupName { get; set; }
        public byte IsGroup { get; set; }
        public int? ParentGroupId { get; set; } 
        public byte IsBudgetApplicable { get; set; }          
    }
}