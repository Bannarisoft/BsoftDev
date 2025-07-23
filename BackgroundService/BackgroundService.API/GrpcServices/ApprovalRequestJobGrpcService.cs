using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Workflow.ApprovalRequests.Commands.CreateApprovalRequest;
using Grpc.Core;
using GrpcServices.Background;
using MediatR;

namespace BackgroundService.API.GrpcServices
{
    public class ApprovalRequestJobGrpcService : ApprovalRequestJobService.ApprovalRequestJobServiceBase
    {
        private readonly IMediator _mediator;
        public ApprovalRequestJobGrpcService(IMediator mediator)
        {
            _mediator = mediator;
        }
           public async override Task<ApprovalRequestResponse> ApprovalRequest(ApprovalRequestDto request, ServerCallContext context)
        {

            var approvalRequest = await _mediator.Send(
                 new CreateApprovalRequestCommand
            {
                ModuleTypeName = request.ModuleTypeName, 
                ModuleTransactionId = request.ModuleTransactionId
            });    
            var response = new ApprovalRequestResponse
            {
                IsSuccess = approvalRequest
            };

            return response;

        }
    }
}