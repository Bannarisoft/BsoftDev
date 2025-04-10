using Core.Domain.Common;

namespace Core.Domain.Entities.WorkOrderMaster    
{
    public class WorkOrder :BaseEntity
    {
        public int CompanyId { get; set; } 
        public int UnitId { get; set; } 
        public int? WorkOrderTypeId { get; set; }
        public MaintenanceCategory CategoryType { get; set; } = null!; 
        public string? RequestId { get; set; }
        public int RequestTypeId { get; set; }          
        public MiscMaster MiscRequestType { get; set; } = null!;         
        public int? MachineId { get; set; }
        public int PriorityId { get; set; }        
        public MiscMaster MiscPriority { get; set; } = null!; 
        public string? Remarks { get; set; }
        public string? Image { get; set; }
        public int StatusId { get; set; }   
        public DateTimeOffset? LastActivityDate { get; set; }   
        public MiscMaster MiscStatus { get; set; } = null!;                
        public int? VendorId { get; set; }      
        public string? OldVendorId { get; set; }      
        public string? VendorName { get; set; }      
        public int RootCauseId { get; set; }          
        public MiscMaster MiscRootCause { get; set; } = null!; 
        
        public ICollection<WorkOrderItem>? Item  {get; set;}  
        public ICollection<WorkOrderActivity>? Activity  {get; set;}  
        public ICollection<WorkOrderSchedule>? Schedule  {get; set;}  
        public ICollection<WorkOrderTechnician>? Technicians {get; set;} 
    }
}