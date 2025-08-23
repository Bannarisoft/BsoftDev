using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Contracts.Interfaces.External.IWorkflow;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IPurchaseIndent;
using Core.Domain.Common;
using MediatR;

namespace Core.Application.PurchaseIndents.Queries.GetPendingIndentById
{
    public class GetPendingIndentByIdQueryHandler : IRequestHandler<GetPendingIndentByIdQuery, PendingIndentByIdDto>
    {
        private readonly IPurchaseIndentQuery _purchaseIndentQuery;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IWorkflowGrpcClient _workflowGrpcClient;
        private readonly IUsersAllGrpcClient _usersAllGrpcClient;
        private readonly IIPAddressService _ipAddressService;
        public GetPendingIndentByIdQueryHandler(IPurchaseIndentQuery purchaseIndentQuery, IMediator mediator, IMapper mapper,
        IWorkflowGrpcClient workflowGrpcClient, IUsersAllGrpcClient usersAllGrpcClient, IIPAddressService ipAddressService)
        {
            _purchaseIndentQuery = purchaseIndentQuery;
            _mediator = mediator;
            _mapper = mapper;
            _workflowGrpcClient = workflowGrpcClient;
            _usersAllGrpcClient = usersAllGrpcClient;
            _ipAddressService = ipAddressService;
        }
        public async Task<PendingIndentByIdDto> Handle(GetPendingIndentByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _purchaseIndentQuery.GetByIdAsync(request.Id);     
               
            var Indent = _mapper.Map<PendingIndentByIdDto>(result);

            var workflowResponse = await _workflowGrpcClient.GetApprovalRequestLineStatusAsync(MiscEnumEntity.PurchaseIndent);
            var workflowApproverResponse = await _workflowGrpcClient.GetApproverListAsync(MiscEnumEntity.PurchaseIndent);

             var ApproverStatusLookup = workflowResponse.ToDictionary(d => d.ModuleLineTransactionId, d => d.Status);
             var ApproverLookup = workflowApproverResponse.ToDictionary(d => d.ModuleLineTransactionId, d => d.ApproverValue);
             var ApproveRequestLineLookup = workflowApproverResponse.ToDictionary(d => d.ModuleLineTransactionId, d => d.ApprovalRequestLineId);

             Indent.ApprovalRequestHeaderId = workflowApproverResponse.Where(d => d.ModuleLineTransactionId == Indent.IndentDetails.FirstOrDefault().Id).FirstOrDefault().ApprovalRequestId;

            foreach (var dto in Indent.IndentDetails)
            {
                if (ApproverStatusLookup.TryGetValue(dto.Id, out var Status))
                {
                    dto.Status = Status;
                }
                if (ApproverLookup.TryGetValue(dto.Id, out var ApproverValue))
                {
                    dto.ApproverId = Convert.ToInt32(ApproverValue);
                }
                if (ApproveRequestLineLookup.TryGetValue(dto.Id, out var ApprovalRequestLineId))
                {
                    dto.ApprovalRequestLineId = ApprovalRequestLineId;
                }
            }

        
            var approverNameMap = await _usersAllGrpcClient.GetUserAllAsync();
            var approverNameLookup = approverNameMap.ToDictionary(d => d.UserId, d => d.UserName);
            foreach (var approverMap in Indent.IndentDetails)
            {
                if (approverNameLookup.TryGetValue(approverMap.ApproverId, out var UserName))
                {
                    approverMap.ApproverName = UserName;
                }
                approverMap.IsApprover = approverMap.ApproverId == _ipAddressService.GetUserId() ? "Y" : "N";
            }
            
            return Indent;
        }
    }
}