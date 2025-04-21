
namespace Core.Application.WorkOrder.Queries.GetWorkOrderById
{
    public class GetWorkOrderCheckListByIdDto
    {
        public int WorkOrderId { get; set; }        
        public int CheckListId { get; set; }
        public string? ActivityCheckList { get; set; }       
        public string? Description { get; set; }
    }
}