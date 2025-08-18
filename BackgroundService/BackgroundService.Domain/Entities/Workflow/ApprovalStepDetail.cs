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
        public byte StopOnFirstMatch { get; set; }
        public int ApprovalStepId { get; set; }
        public WorkflowType WorkflowType { get; set; }
        public MiscMaster ApprovalStep { get; set; }
        public ICollection<ApprovalStepUnitMapping> ApprovalStepUnitMappings { get; set; }
        
        public ICollection<ApprovalRequest> ApprovalRequest { get; set; }
        
    }
}