using Core.Domain.Common;

namespace Core.Domain.Entities.WorkOrderMaster
{
    public class WorkOrderTechnician : BaseEntity
    {
        public int? WorkOrderId { get; set; }
        public WorkOrder WOTechnician { get; set; } = null!; 
        public int TechnicianId { get; set; }
        public string? TechnicianName { get; set; }
        public decimal HoursSpent { get; set; }        
    }
}