using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Entities.Workflow;

namespace BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest
{
    public interface IApprovalRequestCommand
    {
        Task<bool> CreateBulkAsync(string WorkflowType, int TransactionId, string ContextJson);
        Task<bool> Approve(ApprovalRequest approvalRequest);
        Task<bool> Reject(ApprovalRequest approvalRequest);
    }
}