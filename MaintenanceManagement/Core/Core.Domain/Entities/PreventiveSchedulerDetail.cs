using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;
using Core.Domain.Entities.WorkOrderMaster;

namespace Core.Domain.Entities
{

    public class PreventiveSchedulerDetail : BaseEntity
    {

        public int PreventiveSchedulerHeaderId { get; set; }
        public required PreventiveSchedulerHeader PreventiveScheduler { get; set; }
        public int MachineId { get; set; }
        public required MachineMaster Machine { get; set; }

        public DateOnly WorkOrderCreationStartDate { get; set; }
        public DateOnly? ActualWorkOrderDate { get; set; }
        public DateOnly MaterialReqStartDays { get; set; }
        public string? RescheduleReason { get; set; }
        public ICollection<WorkOrder>? workOrdersSchedule  {get; set;}   
        public string? HangfireJobId { get; set; }
        
    }
}