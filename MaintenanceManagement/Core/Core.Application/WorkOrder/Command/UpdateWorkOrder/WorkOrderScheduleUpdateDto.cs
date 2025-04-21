

namespace Core.Application.WorkOrder.Command.UpdateWorkOrder
{
    public class WorkOrderScheduleUpdateDto
    {
        public int? WorkOrderId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }     
    }
}