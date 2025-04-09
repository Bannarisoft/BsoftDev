
namespace Core.Application.WorkOrder.Queries.GetWorkOrderById
{
    public class GetWorkOrderActivityByIdDto
    {
        public int Id { get; set; }                      
        public string? ActivityName { get; set;}
        public string? Description { get; set; }
        public int DepartmentId { get; set; }
        public string? Department { get; set; }        
        public int EstimatedDuration { get; set; }
        public int ActivityType { get; set; }
        public string? ActivityTypeDescription { get; set; }                
    }
}