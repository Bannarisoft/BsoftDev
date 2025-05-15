using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;

namespace SagaOrchestrator.Application.Orchestration.Models.PreventiveSchedule
{
    public class PreventiveSchedulerState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public int PreventiveSchedulerHeaderId { get; set; }
        public int MaintenanceCategoryId { get; set; }
        public int MachineGroupId { get; set; }
        public int FrequencyUnitId { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public int FrequencyInterval { get; set; }
        public int ReminderWorkOrderDays { get; set; }
        public int ReminderMaterialReqDays { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? FailureReason { get; set; }
    }
}