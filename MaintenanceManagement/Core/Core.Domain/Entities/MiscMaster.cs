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
        public ICollection<WorkOrder>? WorkOrderPriority  {get; set;} 
        public ICollection<WorkOrder>? WorkOrderStatus  {get; set;}  
        public ICollection<WorkOrder>? WorkOrderRootCause  {get; set;}  
        public ICollection<PreventiveSchedulerHdr>? Schedule { get; set; }
        public ICollection<PreventiveSchedulerHdr>? DueType { get; set; }
        public ICollection<PreventiveSchedulerHdr>? Frequency { get; set; }
		
  		    
    }
}