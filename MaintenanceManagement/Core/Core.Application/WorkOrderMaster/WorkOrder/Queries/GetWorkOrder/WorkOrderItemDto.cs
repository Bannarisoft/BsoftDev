using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.WorkOrderMaster.WorkOrder.Queries.GetWorkOrder
{
    public class WorkOrderItemDto
    {
        public int? DeptId { get; set; }
        public string? ItemId { get; set; }        
        public int AvailableQty { get; set; }
        public int UsedQty { get; set; }
    }
}