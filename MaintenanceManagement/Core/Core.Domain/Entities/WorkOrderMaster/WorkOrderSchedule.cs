using Core.Domain.Common;

namespace Core.Domain.Entities.WorkOrderMaster
{
    public class WorkOrderSchedule : BaseEntity
    {
        public int? WorkOrderId { get; set; }
        public WorkOrder WOSchedule { get; set; } = null!; 
        public TimeOnly RepairStartTime { get; set; }
        public TimeOnly RepairEndTime { get; set; }
        public TimeOnly? DownTimeStartTime { get; set; }
        public TimeOnly? DownTimeEndTime { get; set; }        
    }
}