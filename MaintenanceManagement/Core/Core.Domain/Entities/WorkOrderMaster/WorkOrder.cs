using Core.Domain.Common;

namespace Core.Domain.Entities.WorkOrderMaster    
{
    public class WorkOrder :BaseEntity
    {
        public int CompanyId { get; set; } 
        public int UnitId { get; set; }         
        public string? WorkOrderDocNo { get; set; }
        public int RequestId { get; set; }
        public MaintenanceRequest WOMaintenanceRequest { get; set; } = null!;     
        public int PreventiveScheduleId { get; set; }                          
        public PreventiveSchedulerDetail WOPreventiveScheduler { get; set; } = null!;     
        public int StatusId { get; set; } 
        public MiscMaster MiscStatus { get; set; } = null!;      
        public int RootCauseId { get; set; }          
        public MiscMaster MiscRootCause { get; set; } = null!;         
        public string? Remarks { get; set; }
        public string? Image { get; set; }
        public int TotalManPower { get; set; }
        public decimal TotalSpentHours { get; set; }        
        public ICollection<WorkOrderItem>? WorkOrderItems  {get; set;}  
        public ICollection<WorkOrderActivity>? WorkOrderActivities  {get; set;}  
        public ICollection<WorkOrderSchedule>? WorkOrderSchedules  {get; set;}  
        public ICollection<WorkOrderTechnician>? WorkOrderTechnicians {get; set;} 
        public ICollection<WorkOrderCheckList>? WorkOrderCheckLists {get; set;} 
    }
}