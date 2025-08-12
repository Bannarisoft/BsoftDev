using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest;
using Grpc.Core;
using GrpcServices.BackgroundService;

namespace BackgroundService.API.GrpcServices
{
    public class ApprovalRequestStatusAllGrpcService : ApprovalRequestStatusAllService.ApprovalRequestStatusAllServiceBase
    {
        private readonly IApprovalRequestQuery _approvalRequestQuery;
        public ApprovalRequestStatusAllGrpcService(IApprovalRequestQuery approvalRequestQuery)
        {
            _approvalRequestQuery = approvalRequestQuery;
        }
        public override async Task<ApprovalStatusAllListResponse> GetApprovalRequestStatusAll(ApprovalStatusRequest request, ServerCallContext context)
        {

            var data = await _approvalRequestQuery.GetAllApprovalRequestByWorkflowType(request.ModuleTypeName);
            var response = new ApprovalStatusAllListResponse();
            
            foreach (var item in data)
              {
                  response.Approvalstatus.Add(new ApprovalRequestStatusDto
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