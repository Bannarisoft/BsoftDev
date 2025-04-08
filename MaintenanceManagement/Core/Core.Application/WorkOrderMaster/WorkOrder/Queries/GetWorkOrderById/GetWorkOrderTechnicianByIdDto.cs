using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.WorkOrderMaster.WorkOrder.Queries.GetWorkOrderById
{
    public class GetWorkOrderTechnicianByIdDto
    {
        public int? TechnicianId { get; set; }
        public int? TechnicianName { get; set; }
        public decimal HoursSpent { get; set; }    
    }
}