using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest;
using Grpc.Core;
using GrpcServices.BackgroundService;

namespace BackgroundService.API.GrpcServices
{
    public class ApprovedApprovalRequestGrpcService : ApprovedApprovalRequestService.ApprovedApprovalRequestServiceBase
    {
        private readonly IApprovalRequestQuery _approvalRequestQuery;
        public ApprovedApprovalRequestGrpcService(IApprovalRequestQuery approvalRequestQuery)
        {
            _approvalRequestQuery = approvalRequestQuery;
        }
           public override async Task<ApprovedTransactionIdListResponse> GetApprovedApprovalRequest(ApprovedApprovalRequest request, ServerCallContext context)
        {

            var data = await _approvalRequestQuery.GetAllApprovalRequestByApproved(request.ModuleTypeName);
            var response = new ApprovedTransactionIdListResponse();

             response.ModuleTransactionIds.AddRange(data);

            return response;

        }
    }
}