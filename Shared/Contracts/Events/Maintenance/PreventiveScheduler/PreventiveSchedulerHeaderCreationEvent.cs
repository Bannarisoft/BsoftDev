using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;

namespace Contracts.Events.Maintenance.PreventiveScheduler
{
    public class PreventiveSchedulerHeaderCreationEvent : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
         public int PreventiveSchedulerHeaderId { get; set; }
         public int MaintenanceCategoryId { get; set; }
         public int MachineGroupId { get; set; }
         public int FrequencyUnitId { get; set; }
         public DateOnly EffectiveDate { get; set; }
         public int FrequencyInterval { get; set; }
         public int ReminderWorkOrderDays { get; set; }
         public int ReminderMaterialReqDays { get; set; }
    }
}