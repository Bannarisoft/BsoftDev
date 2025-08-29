using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Dtos.Purchase;
using MassTransit;

namespace SagaOrchestrator.Application.Orchestration.Models.Workflow
{
    public class ApprovedRejectedState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public int IndentId { get; set; }
        public string Status { get; set; }
        public ICollection<UpdateApprovedQtyDto> ApprovedQty { get; set; }
    }
}