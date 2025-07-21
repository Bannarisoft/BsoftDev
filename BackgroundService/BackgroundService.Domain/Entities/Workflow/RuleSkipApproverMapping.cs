using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundService.Domain.Entities.Workflow
{
    public class RuleSkipApproverMapping
    {
        public int Id { get; set; }
        public int RuleId { get; set; }
        public int ApprovalDetailId { get; set; }
        public ApprovalStepDetail ApprovalStepDetail { get; set; }
    }
}