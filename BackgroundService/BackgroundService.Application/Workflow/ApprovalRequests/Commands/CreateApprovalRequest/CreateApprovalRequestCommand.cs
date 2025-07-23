using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BackgroundService.Application.Workflow.ApprovalRequests.Commands.CreateApprovalRequest
{
    public class CreateApprovalRequestCommand : IRequest<bool>
    {
        public string ModuleTypeName { get; set; }
        public int WorkflowTypeId { get; set; }
        public int ModuleTransactionId { get; set; }
        public int ApprovalStepDetailId { get; set; }
        public int ApprovalRuleId { get; set; }
        public int StatusId { get; set; }
        public DateTimeOffset RequestedDate { get; set; }
    }
}