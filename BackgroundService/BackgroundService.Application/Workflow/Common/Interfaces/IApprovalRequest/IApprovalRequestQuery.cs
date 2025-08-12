using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Entities.Workflow;

namespace BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest
{
    public interface IApprovalRequestQuery
    {
        Task<List<int>> GetApprovalStepDetailByIdAsync(int WorkFlowTypeId, int ModuleTransactionId, int UnitId, int DepartmentId);
        Task<(List<ApprovalRequest>, int)> GetAllApprovalRequestAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<List<dynamic>> GetAllApprovalRequestByWorkflowType(string ModuleTypeName);
        Task<List<dynamic>> GetAllApprovalRequestByApprover(string ModuleTypeName, int ApproverId);
        Task<List<int>> GetAllApprovalRequestByApproved(string ModuleTypeName);
        Task<List<int>> StartApprovalProcessAsync(List<int> Id, Dictionary<string, object> requestData);
    }
}