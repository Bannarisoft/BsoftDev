
namespace Core.Application.WorkOrder.Queries.GetWorkOrderById
{
    public class GetWorkOrderScheduleByIdDto
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }        
    }
}