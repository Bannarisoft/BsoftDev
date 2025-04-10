
namespace Core.Application.WorkOrder.Queries.GetWorkOrder
{
    public class WorkOrderTechnicianDto
    {       
        public int? TechnicianId { get; set; }
        public string? TechnicianName { get; set; }
        public decimal HoursSpent { get; set; }          
    }
}