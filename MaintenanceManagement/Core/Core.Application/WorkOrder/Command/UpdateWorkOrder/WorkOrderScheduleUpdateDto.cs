

namespace Core.Application.WorkOrder.Command.UpdateWorkOrder
{
    public class WorkOrderScheduleUpdateDto
    {
        public int? WorkOrderId { get; set; }
        public DateTimeOffset RepairStartTime { get; set; }
        public DateTimeOffset RepairEndTime { get; set; }     
    }
}