
using Core.Domain.Common;

namespace Core.Domain.Entities.WorkOrderMaster
{
    public class WorkOrderItem : BaseEntity
    {
        public int? WorkOrderId { get; set; }
        public WorkOrder WOItem { get; set; } = null!; 
        public int? DeptId { get; set; }
        public string? ItemCode { get; set; }        
        public int AvailableQty { get; set; }
        public int UsedQty { get; set; }
        public string? Image { get; set; }
    }
}
