using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundService.Application.Workflow.ApprovalRules.Queries.GetApprovalRuleAutoComplete
{
    public class ApprovalRuleAutoCompleteDto
    {
        public int Id { get; set; }
        public string ConditionKey { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }
}