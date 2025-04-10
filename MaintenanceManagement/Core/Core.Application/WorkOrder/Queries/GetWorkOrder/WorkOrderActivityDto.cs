
namespace Core.Application.WorkOrder.Queries.GetWorkOrder
{
    public class WorkOrderActivityDto
    {   
        public int ActivityId { get; set; }
        public Decimal EstimatedTime { get; set; }
        public string? Description { get; set; }        
    }
}