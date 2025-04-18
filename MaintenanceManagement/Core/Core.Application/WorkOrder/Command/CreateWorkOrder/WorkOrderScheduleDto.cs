
namespace Core.Application.WorkOrder.Command.CreateWorkOrder
{
    public class WorkOrderScheduleDto
    {        
        public DateTimeOffset RepairStartTime { get; set; }
        public DateTimeOffset RepairEndTime { get; set; }        
    }
}