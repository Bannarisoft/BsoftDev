
namespace Core.Application.WorkOrder.Queries.GetWorkOrder
{
    public class WorkOrderItemDto
    {
        public int? DepartmentId { get; set; }
        public string? ItemCode { get; set; }   
        public string? ItemName { get; set; }        
        public int AvailableQty { get; set; }
        public int UsedQty { get; set; }
        public string? Image { get; set; }
    }
}