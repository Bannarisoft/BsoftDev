using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest
{
    public interface IApprovalRequestQuery
    {
        Task<int> GetApprovalStepDetailByIdAsync(int WorkFlowTypeId,int ModuleTransactionId);
    }
}