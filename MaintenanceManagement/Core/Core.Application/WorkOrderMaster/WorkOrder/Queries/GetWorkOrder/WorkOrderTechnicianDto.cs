
namespace Core.Application.WorkOrderMaster.WorkOrder.Queries.GetWorkOrder
{
    public class WorkOrderTechnicianDto
    {       
        public int? TechnicianId { get; set; }
        public decimal HoursSpent { get; set; }
        public int RootCauseId { get; set; }     
    }
}