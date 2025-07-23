using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundService.Application.Workflow.ApprovalRules.Queries.GetAllApprovalRule
{
    public class ApprovalRuleDto
    {
        public int Id { get; set; }
        public string ConditionKey { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public string Action { get; set; }
        public int UnitId { get; set; }
        public int WorkflowTypeId { get; set; }
        public byte IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string CreatedByName { get; set; }
        public int ModifiedBy { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        public string ModifiedByName { get; set; }
        public WorkflowTypeDto WorkflowType { get; set; }
    }
}