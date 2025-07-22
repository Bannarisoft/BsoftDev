using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundService.Application.Workflow.ApprovalStepDetails.Commands.CreateApprovalStepDetail
{
    public class RuleSkipApproverMappingDto
    {
        public int RuleId { get; set; }
        public int ApprovalDetailId { get; set; }
    }
}