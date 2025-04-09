
namespace Core.Application.WorkOrder.Queries.GetWorkOrderById
{
    public class GetWorkOrderTechnicianByIdDto
    {
        public int? TechnicianId { get; set; }
        public int? TechnicianName { get; set; }
        public decimal HoursSpent { get; set; }    
    }
}