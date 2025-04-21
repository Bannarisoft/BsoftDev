
namespace Core.Application.WorkOrder.Queries.GetWorkOrderById
{
    public class GetWorkOrderScheduleByIdDto
    {
        public string? RequestId { get; set; }
        public string? WorkOrderId { get; set; }
        public string? DepartmentName { get; set; }
        public string? MachineCode { get; set; }
        public string? MachineName { get; set; }
        public DateTimeOffset RequestDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public TimeOnly DownTimeStartTime { get; set; }
        public TimeOnly DownTimeEndTime { get; set; }       
        public TimeOnly DifferenceHrs { get; set; }       
    }
}