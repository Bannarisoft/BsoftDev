using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BackgroundService.Application.Workflow.ApprovalRequests.Commands.RejectApprovalRequest
{
    public class RejectApprovalRequestCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}