using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Common;
using BackgroundService.Domain.Entities.Notification;

namespace BackgroundService.Domain.Entities.Workflow
{
    public class ApprovalStepDetail : BaseEntity
    {
        public int WorkFlowTypeId { get; set; }
        public int StepOrder { get; set; }
        public int TargetTypeId { get; set; }
        public int ApprovalStepId { get; set; }
        public int ApprovalTypeId { get; set; }
        public decimal? SLAHours { get; set; }
        public string? OnSLAAction { get; set; }
        public WorkflowType WorkflowType { get; set; }
        public MiscMaster ApprovalStep { get; set; }
        public MiscMaster ApprovalType { get; set; }
        public ICollection<RuleSkipApproverMapping> RuleSkipApproverMappings { get; set; }
        public ICollection<ApprovalStepUnitMapping> ApprovalStepUnitMappings { get; set; }
        public ICollection<ApprovalRequest> ApprovalRequest { get; set; } 
        public ICollection<ApprovalStepDepartmentMapping> ApprovalStepDepartmentMappings { get; set; }
        
    }
}