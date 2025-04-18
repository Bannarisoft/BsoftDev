
using Core.Domain.Common;

namespace Core.Domain.Entities.WorkOrderMaster
{
    public class WorkOrderItem 
    {
        public int Id { get; set; }
        public int? WorkOrderId { get; set; }
        public WorkOrder WOItem { get; set; } = null!; 
        public int? DepartmentId { get; set; }
        public int ItemId { get; set; }        
        public int OldItemId { get; set; }         
        public string? ItemName { get; set; }  
        public int SourceId { get; set; }   
        public int AvailableQty { get; set; }
        public int UsedQty { get; set; }
        public string? Image { get; set; }
    }
}
