using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Common;
using BackgroundService.Domain.Entities.Notification;

namespace BackgroundService.Domain.Entities.Workflow
{
    public class ApprovalRule : BaseEntity
    {

        public int WorkflowTypeId { get; set; }
        public int ApprovalStepId { get; set; }
        public int Priority { get; set; }
        public string? Action { get; set; }
        public int UnitId { get; set; }
        public DateOnly EffectiveFrom { get; set; }
        public DateOnly EffectiveTo { get; set; }
        public WorkflowType WorkflowType { get; set; }
        public ICollection<ApprovalRequest> ApprovalRequest { get; set; }
        public MiscMaster ApprovalStep { get; set; }
        public ICollection<ApprovalRuleCondition> Conditions { get; set; }
        public ICollection<RuleTargetOverride> RuleTargetOverride { get; set; }
    }
}