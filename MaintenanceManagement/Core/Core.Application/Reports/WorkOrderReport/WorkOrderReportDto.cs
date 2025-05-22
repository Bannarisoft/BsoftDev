namespace Core.Application.Reports.WorkOrderReport
{
    public class WorkOrderReportDto
    {
        public string? WODate { get; set; }
        public string? WorkOrderDocNo { get; set; } 
        public string? CreatedUser { get; set; } 
        public string? MaintenanceType { get; set; } 
        public string? Status { get; set; } 
        public string? Machine { get; set; } 
        public string? MachineName { get; set; } 
        public int RequestId { get; set; } 
        public string? DowntimeStart { get; set; } 
        public string? DowntimeEnd { get; set; } 
        public string? TotalDownTime { get; set; } 
        public string? MaintenanceStartTime { get; set; } 
        public string? MaintenanceEndTime { get; set; } 
        public string? TotalMaintenanceTime { get; set; } 
        public string? Department { get; set; } 
        public decimal ItemCost { get; set; } 
    }
}