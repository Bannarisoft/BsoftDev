namespace Core.Application.Dashboard.ItemConsumption
{
    public class ItemConsumptionDto
    {
        public string? ItemName { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; } 
        public decimal IssueQty { get; set; }
    }
}
