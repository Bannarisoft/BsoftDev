using Core.Domain.Common;
using Core.Domain.Entities.WorkOrderMaster;



namespace Core.Domain.Entities
{
    public class MiscMaster  :BaseEntity
    {
        
         public int MiscTypeId { get; set; }  
        public string? Code { get; set;}
        public string? Description { get; set;}
        public int SortOrder  { get; set;}
        public Status IsActive { get; set; }
            
        public MiscTypeMaster? MiscTypeMaster { get; set; } 
        public ICollection<MaintenanceRequest>? RequestType { get; set; }
        public ICollection<MaintenanceRequest>? MaintenanceType { get; set; }
        public ICollection<WorkOrder>? WorkOrderPriority  {get; set;} 
        public ICollection<WorkOrder>? WorkOrderStatus  {get; set;}  
        public ICollection<WorkOrder>? WorkOrderRootCause  {get; set;}  
        public ICollection<WorkOrder>? WorkOrderRequestType  {get; set;}  
        public ICollection<PreventiveSchedulerHeader>? Schedule { get; set; }
        public ICollection<PreventiveSchedulerHeader>? FrequencyType { get; set; }
        public ICollection<PreventiveSchedulerHeader>? FrequencyUnit { get; set; }      
        public ICollection<MaintenanceRequest>? ServiceType { get; set; }
        public ICollection<MaintenanceRequest>? ServiceLocation { get; set; }
        public ICollection<MaintenanceRequest>? SpareType { get; set; }

        public ICollection<MaintenanceRequest>? RequestStatus { get; set; }

        public ICollection<MaintenanceRequest>? ModeOfDispatchType { get; set; }




		
  		    
    }
}