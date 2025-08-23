using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Entities.Workflow;

namespace BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest
{
    public interface IApprovalRequestGrpcQuery
    {
        Task<List<ApprovalRequest>> GetApprovalRequestByWorkFlowTypeAsync(string WorkFlowType);
        Task<List<ApprovalRequestLine>> GetApproverListByWorkFlowTypeAsync(string WorkFlowType);
        Task<List<dynamic>> ApprovalRequestLineStatusByWorkFlowType(string WorkFlowType);
    }
}