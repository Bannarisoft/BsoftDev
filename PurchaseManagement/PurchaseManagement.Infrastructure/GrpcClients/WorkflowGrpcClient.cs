using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Dtos.Workflow;
using Contracts.Interfaces.External.IWorkflow;
using Grpc.Core;
using GrpcServices.BackgroundService;
using GrpcServices.BackgroundService.Line;
using Microsoft.AspNetCore.Http;

namespace PurchaseManagement.Infrastructure.GrpcClients
{
    public class WorkflowGrpcClient : IWorkflowGrpcClient
    {
        private readonly ApprovalRequestStatusAllService.ApprovalRequestStatusAllServiceClient _approvalRequestStatusAllService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApprovalRequestLineStatusService.ApprovalRequestLineStatusServiceClient _approvalRequestLineStatusService;
        public WorkflowGrpcClient(ApprovalRequestStatusAllService.ApprovalRequestStatusAllServiceClient approvalRequestStatusAllService,
        IHttpContextAccessor httpContextAccessor, ApprovalRequestLineStatusService.ApprovalRequestLineStatusServiceClient approvalRequestLineStatusService)
        {
            _approvalRequestStatusAllService = approvalRequestStatusAllService;
            _httpContextAccessor = httpContextAccessor;
            _approvalRequestLineStatusService = approvalRequestLineStatusService;
        }
        public Task<List<int>> GetAllApprovalRequestByApproved(string ModuleTypeName)
        {
            throw new NotImplementedException();
        }

        public Task<List<ApprovalByApproverDto>> GetAllApprovalRequestByApprover(string ModuleTypeName, int ApproverId)
        {
            throw new NotImplementedException();
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
             var request = new GrpcServices.BackgroundService.ApprovalStatusRequest { ModuleTypeName = ModuleTypeName };

            var response = await _approvalRequestStatusAllService.GetApprovalRequestStatusAllAsync(request, new CallOptions(metadata));

            return response.Approvalstatus.Select(u => new Contracts.Dtos.Workflow.ApprovalRequestStatusDto
            {
                ModuleTransactionId = u.ModuleTransactionId,
                CurrentStatus = u.CurrentStatus
            }).ToList();
        }

        public async Task<List<Contracts.Dtos.Workflow.ApprovalRequestLineStatusDto>> GetApprovalRequestLineStatusAsync(string ModuleTypeName)
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
             var request = new GrpcServices.BackgroundService.Line.ApprovalStatusRequest { ModuleTypeName = ModuleTypeName };

            var response = await _approvalRequestLineStatusService.GetApprovalRequestLineStatusAsync(request, new CallOptions(metadata));

            return response.Approvalstatus.Select(u => new Contracts.Dtos.Workflow.ApprovalRequestLineStatusDto
            {
                ModuleLineTransactionId = u.ModuleLineTransactionId,
                Status = u.Status,
                ApproverBinding = u.ApproverBinding,
                ApproverValue = u.ApproverValue,
                ApprovalRequestId = u.ApprovalRequestId,
                ApprovalRequestLineId = u.ApprovalRequestLineId
            }).ToList();
        }
    }
}