using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.WorkOrder.Command.UpdateWorkOrder
{
    public class WorkOrderCheckListUpdateDto
    {
        public int? WorkOrderId { get; set; }
        public int CheckListId { get; set; }   
        public string? Description { get; set; }   
    }
}