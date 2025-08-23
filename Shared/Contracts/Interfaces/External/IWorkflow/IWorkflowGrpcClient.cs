using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Dtos.Workflow;

namespace Contracts.Interfaces.External.IWorkflow
{
    public interface IWorkflowGrpcClient
    {
        Task<List<ApprovalRequestStatusDto>> GetAllApprovalRequestStatusAsync(string ModuleTypeName);
        Task<List<ApprovalRequestLineStatusDto>> GetApprovalRequestLineStatusAsync(string ModuleTypeName);
        Task<List<ApproverListDto>> GetApproverListAsync(string ModuleTypeName);
    }
}