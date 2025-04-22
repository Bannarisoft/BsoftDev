
namespace Core.Application.WorkOrder.Queries.GetWorkOrderById
{
    public class GetWorkOrderTechnicianByIdDto
    {
        public int? TechnicianId { get; set; }
        public int? OldTechnicianId { get; set; }
        public string? TechnicianName { get; set; }
        public int? SourceId { get; set; }   
        public string? SourceDesc { get; set; }   
        public int HoursSpent { get; set; }    
        public int MinutesSpent { get; set; } 
    }

}