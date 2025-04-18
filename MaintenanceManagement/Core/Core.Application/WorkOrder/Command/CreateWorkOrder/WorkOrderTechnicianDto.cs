
namespace Core.Application.WorkOrder.Command.CreateWorkOrder
{
    public class WorkOrderTechnicianDto
    {       
        public int? TechnicianId { get; set; }
        public int OldTechnicianId { get; set; }
        public int SourceId { get; set; }        
        public string? TechnicianName { get; set; }
        public decimal HoursSpent { get; set; }          
    }
}