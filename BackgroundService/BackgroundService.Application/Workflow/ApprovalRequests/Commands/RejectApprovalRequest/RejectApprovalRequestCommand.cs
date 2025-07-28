using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Workflow.ApprovalRequests.Commands.ApproveApprovalRequest;
using MediatR;

namespace BackgroundService.Application.Workflow.ApprovalRequests.Commands.RejectApprovalRequest
{
    public class RejectApprovalRequestCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Remark { get; set; }
        public ICollection<ApprovalDocumentDto>? ApprovalDocument { get; set; }
    }
}