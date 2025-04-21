using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities.WorkOrderMaster;

namespace Core.Domain.Entities
{
    public class PreventiveSchedulerDetail
    {
        public int Id { get; set; }
        public int PreventiveSchedulerId { get; set; }
        public required PreventiveSchedulerHeader PreventiveScheduler { get; set; }
        public int MachineId { get; set; }
        public required MachineMaster Machine { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? NextDueDate { get; set; }
        public string? RescheduleReason { get; set; }
        public ICollection<WorkOrder>? workOrdersSchedule  {get; set;}   
        
    }
}