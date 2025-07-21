using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundService.Application.Workflow.ApprovalStepDetails.Queries.GetAllApprovalStepDetail
{
    public class ApprovalStepDetailDto
    {
        public int WorkFlowTypeId { get; set; }
        public int StepOrder { get; set; }
        public int TargetTypeId { get; set; }
        public int ApprovalStepId { get; set; }
        public int ApprovalTypeId { get; set; }
        public decimal SLAHours { get; set; }
        public string OnSLAAction { get; set; }
        public WorkflowTypeApprovalStepDto WorkflowType { get; set; }
        public ApprovalStepDto ApprovalStep { get; set; }
        public ApprovalTypeDto ApprovalType { get; set; }
    }
}