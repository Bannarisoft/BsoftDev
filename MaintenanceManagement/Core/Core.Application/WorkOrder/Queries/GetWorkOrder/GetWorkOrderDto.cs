
namespace Core.Application.WorkOrder.Queries.GetWorkOrder
{
    public class ScheduleDto
    {
        public DateTimeOffset? Start { get; set; }
        public DateTimeOffset? End { get; set; }
        public byte ISCompleted { get; set; } 
    }
    public class GetWorkOrderDto
    {
        public int Id { get; set; }                   
        public string? WorkOrderDocNo { get; set; }
        public string? Department { get; set; }
        public string? Machine { get; set; }
        public string? RequestDate { get; set; }
        public string? RequestType { get; set; }
        public string? Status { get; set; }   
        public string? MaintenanceType { get; set; }  
        public int RequestId { get; set; }
        public List<ScheduleDto> Schedules { get; set; } = new();
    }
}