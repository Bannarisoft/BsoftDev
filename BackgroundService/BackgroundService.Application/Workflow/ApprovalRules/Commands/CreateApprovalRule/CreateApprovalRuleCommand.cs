using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BackgroundService.Application.Workflow.ApprovalRules.Commands.CreateApprovalRule
{
    public class CreateApprovalRuleCommand : IRequest<int>
    {
        public string ConditionKey { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public string Action { get; set; }
        public int UnitId { get; set; }
        public int WorkflowTypeId { get; set; }
    }
}