using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;

namespace SagaOrchestrator.Application.Orchestration.Models.Workflow
{
    public class ApprovalRequestState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public string ModuleTypeName { get; set; }
        public int ModuleTransactionId { get; set; }
        public string Payload { get; set; }
    }
}