using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Dtos.Workflow;
using Contracts.Interfaces.External.IWorkflow;
using Grpc.Core;
using GrpcServices.BackgroundService;
using Microsoft.AspNetCore.Http;

namespace FAM.Infrastructure.GrpcClients
{
    public class WorkflowGrpcClient : IWorkflowGrpcClient
    {
        private readonly ApprovalRequestStatusAllService.ApprovalRequestStatusAllServiceClient _client;
        private readonly ApprovalRequestByApproverService.ApprovalRequestByApproverServiceClient _clientByApprover;
        private readonly ApprovedApprovalRequestService.ApprovedApprovalRequestServiceClient _approvedList;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public WorkflowGrpcClient(ApprovalRequestStatusAllService.ApprovalRequestStatusAllServiceClient client, IHttpContextAccessor httpContextAccessor,
            ApprovalRequestByApproverService.ApprovalRequestByApproverServiceClient clientByApprover,
            ApprovedApprovalRequestService.ApprovedApprovalRequestServiceClient approvedList)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
            _clientByApprover = clientByApprover;
            _approvedList = approvedList;
            
        }

        public async Task<List<int>> GetAllApprovalRequestByApproved(string ModuleTypeName)
        {
            var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("Authorization token not found.");

            if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                token = $"Bearer {token}";

            var metadata = new Metadata
            {
                { "Authorization", token }
            };
            var request = new ApprovedApprovalRequest { ModuleTypeName = ModuleTypeName };

            var response = await _approvedList.GetApprovedApprovalRequestAsync(request, new CallOptions(metadata));
             
             return response.ModuleTransactionIds.ToList();
        }

        public async Task<List<Contracts.Dtos.Workflow.ApprovalByApproverDto>> GetAllApprovalRequestByApprover(string ModuleTypeName, int ApproverId)
        {
             var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("Authorization token not found.");

            if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                token = $"Bearer {token}";

            var metadata = new Metadata
            {
                { "Authorization", token }
            };
            var request = new ApprovalRequestByApprover { ModuleTypeName = ModuleTypeName, ApproverId = ApproverId };
            
             var response = await _clientByApprover.GetApprovalRequestByApproverAsync(request, new CallOptions(metadata));

            return response.Approvalstatus.Select(u => new Contracts.Dtos.Workflow.ApprovalByApproverDto
            {
                ModuleTransactionId = u.ModuleTransactionId,
                ApprovalRequestId = u.ApprovalRequestId,
                CurrentStatus = u.CurrentStatus,
                ModuleTypeName = u.ModuleTypeName
            }).ToList();
        }

        public async Task<List<Contracts.Dtos.Workflow.ApprovalRequestStatusDto>> GetAllApprovalRequestStatusAsync(string ModuleTypeName)
        {
            var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("Authorization token not found.");

            if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                token = $"Bearer {token}";

            var metadata = new Metadata
            {
                { "Authorization", token }
            };
            var request = new ApprovalStatusRequest { ModuleTypeName = ModuleTypeName };
            
             var response = await _client.GetApprovalRequestStatusAllAsync(request, new CallOptions(metadata));

            return response.Approvalstatus.Select(u => new Contracts.Dtos.Workflow.ApprovalRequestStatusDto
            {
                ModuleTransactionId = u.ModuleTransactionId,
                ApprovalRequestId = u.ApprovalRequestId,
                CurrentStatus = u.CurrentStatus,
                ModuleTypeName = u.ModuleTypeName
            }).ToList();
        }
    }
}