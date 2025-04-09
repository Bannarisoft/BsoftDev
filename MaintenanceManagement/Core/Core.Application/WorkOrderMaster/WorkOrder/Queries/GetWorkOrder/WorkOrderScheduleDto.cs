
namespace Core.Application.WorkOrderMaster.WorkOrder.Queries.GetWorkOrder
{
    public class WorkOrderScheduleDto
    {        
        public TimeOnly RepairStartTime { get; set; }
        public TimeOnly RepairEndTime { get; set; }
        public TimeOnly DownTimeStartTime { get; set; }
        public TimeOnly DownTimeEndTime { get; set; }   
    }
}