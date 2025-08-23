using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundService.Application.Workflow.ApprovalRequests.Commands.ApproveApprovalRequest
{
    public class ApproveApprovalRequestLineDto
    {
        public int Id { get; set; }
        public int ApprovalRequestId { get; set; }
        public int ModuleLineTransactionId { get; set; }
        // public string ApproverBinding { get; set; }
        // public string ApproverValue { get; set; }
        // public int StatusId { get; set; }
        public string Remark { get; set; }
        public byte IsApproved { get; set; }
    }
}