using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Entities.Workflow;

namespace BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest
{
    public interface IApprovalRequestQuery
    {
        Task<int?> GetApprovalStepDetailByIdAsync(int WorkFlowTypeId, int ModuleTransactionId,int UnitId,int DepartmentId);
        Task<(List<ApprovalRequest>, int)> GetAllApprovalRequestAsync(int PageNumber, int PageSize, string? SearchTerm);
    }
}