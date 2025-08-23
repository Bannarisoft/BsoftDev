using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BackgroundService.Application.Workflow.ApprovalRequests.Commands.ApproveApprovalRequest
{
    public class ApproveApprovalRequestCommand : IRequest<bool>
    {
        public int Id { get; set; }
        // public int WorkFlowTypeId { get; set; }
        public int ModuleTransactionId { get; set; }
        // public string ModuleTypeName { get; set; }
        // public int UnitId { get; set; }
        // public int DepartmentId { get; set; }
        public string Remark { get; set; }
        public byte IsApproved { get; set; }
        public ICollection<ApprovalDocumentDto>? ApprovalDocument { get; set; }
        public required ICollection<ApproveApprovalRequestLineDto> ApprovalRequestLine { get; set; }
    }
}