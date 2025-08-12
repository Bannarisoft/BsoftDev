using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest;
using Grpc.Core;
using GrpcServices.BackgroundService;

namespace BackgroundService.API.GrpcServices
{
    public class ApprovalRequestByApproverGrpcService : ApprovalRequestByApproverService.ApprovalRequestByApproverServiceBase
    {
        private readonly IApprovalRequestQuery _approvalRequestQuery;
        public ApprovalRequestByApproverGrpcService(IApprovalRequestQuery approvalRequestQuery)
        {
            _approvalRequestQuery = approvalRequestQuery;
        }
          public override async Task<ApprovalByApproverResponse> GetApprovalRequestByApprover(ApprovalRequestByApprover request, ServerCallContext context)
        {

            var data = await _approvalRequestQuery.GetAllApprovalRequestByApprover(request.ModuleTypeName,request.ApproverId);
            var response = new ApprovalByApproverResponse();

            foreach (var item in data)
            {
                response.Approvalstatus.Add(new ApprovalByApproverDto
                {
                    ModuleTransactionId = Convert.ToInt32(item.ModuleTransactionId),
                    ApprovalRequestId = Convert.ToInt32(item.ApprovalRequestId),
                    CurrentStatus = item.CurrentStatus?.ToString() ?? string.Empty,
                    ModuleTypeName = item.ModuleTypeName?.ToString() ?? string.Empty
                });
            }

            return response;

        }
    }
}