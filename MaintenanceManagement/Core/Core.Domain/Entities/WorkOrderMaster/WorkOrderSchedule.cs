using Core.Domain.Common;

namespace Core.Domain.Entities.WorkOrderMaster
{
    public class WorkOrderSchedule 
    {
        public int Id { get; set; }
        public int WorkOrderId { get; set; }
        public WorkOrder WOSchedule { get; set; } = null!; 
        public DateTimeOffset RepairStartTime { get; set; }
        public DateTimeOffset RepairEndTime { get; set; }   
    }
}