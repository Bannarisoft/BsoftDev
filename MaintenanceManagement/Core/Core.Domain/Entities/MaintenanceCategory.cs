using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;
using Core.Domain.Entities.WorkOrderMaster;

namespace Core.Domain.Entities
{
    public class MaintenanceCategory : BaseEntity
    {
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        // public ICollection<WorkOrder>? WorkOrderType  {get; set;} 
    }
}