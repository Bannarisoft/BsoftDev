
namespace Core.Application.WorkOrder.Queries.GetWorkOrderById
{
    public class GetWorkOrderCheckListByIdDto
    {
        public int CheckListId { get; set; }
        public string? CheckListDesc { get; set; }       
        public byte ISCompleted { get; set; }  
        public string? Description { get; set; }
    }
}