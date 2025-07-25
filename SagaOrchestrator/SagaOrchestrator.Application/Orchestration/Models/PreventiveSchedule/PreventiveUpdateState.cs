using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Dtos.Maintenance.Preventive;
using MassTransit;

namespace SagaOrchestrator.Application.Orchestration.Models.PreventiveSchedule
{
    public class PreventiveUpdateState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public int PreventiveSchedulerHeaderId { get; set; }
        public List<MachinedetailDto> MachineDetailDtos { get; set; }
        public int UnitId { get; set; }
        public int FrequencyUnitId { get; set; }
        public int FrequencyInterval { get; set; }
        public int ReminderWorkOrderDays { get; set; }
        public int ReminderMaterialReqDays { get; set; }
        public RollbackHeaderDto rollbackHeaders { get; set; }
    }
}