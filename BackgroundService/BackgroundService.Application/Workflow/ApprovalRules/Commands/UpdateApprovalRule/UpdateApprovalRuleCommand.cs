using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BackgroundService.Application.Workflow.ApprovalRules.Commands.UpdateApprovalRule
{
    public class UpdateApprovalRuleCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string ConditionKey { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public string Action { get; set; }
        public int UnitId { get; set; }
        public int WorkflowTypeId { get; set; }
        public byte IsActive { get; set; }
    }
}