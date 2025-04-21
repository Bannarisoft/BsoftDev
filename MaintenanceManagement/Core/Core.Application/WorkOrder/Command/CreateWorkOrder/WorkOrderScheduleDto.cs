
namespace Core.Application.WorkOrder.Command.CreateWorkOrder
{
    public class WorkOrderScheduleDto
    {        
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }        
    }
}