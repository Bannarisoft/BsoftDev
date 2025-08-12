using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundService.Application.Workflow.ApprovalRequests.Queries.GetAllApprovalRequest
{
    public class ApprovalRequestDto
    {
        public int Id { get; set; }
        public int WorkflowTypeId { get; set; }
        public int ModuleTransactionId { get; set; }
        public int ApprovalStepDetailId { get; set; }
        public int ApprovalRuleId { get; set; }
        public int StatusId { get; set; }
        public DateTimeOffset RequestedDate { get; set; }
        public int TargetTypeId { get; set; }
        public string ApproverName { get; set; }
        public string Status { get; set; }
        public string ModuleTypeName { get; set; }
    }
}