using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.WorkOrder.Command.UpdateWorkOrder
{
    public class WorkOrderItemUpdateDto
    {
        public int? WorkOrderId { get; set; }
        public int? DepartmentId { get; set; }
        public int ItemId { get; set; }   
        public int OldItemId { get; set; }   
        public int SourceId { get; set; }   
        public string? ItemName { get; set; }        
        public int AvailableQty { get; set; }
        public int UsedQty { get; set; }
        public string? Image { get; set; }
    }
}