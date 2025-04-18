using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.WorkOrder.Command.UpdateWorkOrder
{
    public class WorkOrderActivityUpdateDto
    {
        public int WorkOrderId { get; set; }
        public int ActivityId { get; set; }        
        public string? Description { get; set; }       
    }
}