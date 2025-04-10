using Core.Domain.Common;

namespace Core.Domain.Entities.WorkOrderMaster
{
    public class WorkOrderActivity : BaseEntity
    {
        public int? WorkOrderId { get; set; }
        public WorkOrder WOActivity { get; set; } = null!; 
        public int ActivityId { get; set; }
        public ActivityMaster ActivityMaster { get; set; } = null!;
        public Decimal EstimatedTime { get; set; }
        public string? Description { get; set; }
    }
}