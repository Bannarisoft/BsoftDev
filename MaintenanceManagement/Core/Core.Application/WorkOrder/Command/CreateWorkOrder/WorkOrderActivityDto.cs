
namespace Core.Application.WorkOrder.Command.CreateWorkOrder
{
    public class WorkOrderActivityDto
    {   
        public int WorkOrderId { get; set; }
        public int ActivityId { get; set; }        
        public string? Description { get; set; }        
    }
}