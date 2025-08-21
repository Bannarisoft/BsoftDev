using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Contracts.Interfaces.External.IWorkflow;
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
        public GetPendingIndentByIdQueryHandler(IPurchaseIndentQuery purchaseIndentQuery, IMediator mediator, IMapper mapper,
        IWorkflowGrpcClient workflowGrpcClient, IUsersAllGrpcClient usersAllGrpcClient)
        {
            _purchaseIndentQuery = purchaseIndentQuery;
            _mediator = mediator;
            _mapper = mapper;
            _workflowGrpcClient = workflowGrpcClient;
            _usersAllGrpcClient = usersAllGrpcClient;
        }
        public async Task<PendingIndentByIdDto> Handle(GetPendingIndentByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _purchaseIndentQuery.GetByIdAsync(request.Id);     
               
            var Indent = _mapper.Map<PendingIndentByIdDto>(result);

            var workflowResponse = await _workflowGrpcClient.GetApprovalRequestLineStatusAsync(MiscEnumEntity.PurchaseIndent);
           var statusOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    [MiscEnumEntity.Pending]  = 0,
                    ["Rejected"] = 1,
                    ["Approved"] = 2
                };

            var byLine = workflowResponse
                .GroupBy(r => r.ModuleLineTransactionId)
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        Status = g.Select(x => x.Status)
                                  .OrderBy(s => statusOrder.TryGetValue(s ?? "", out var rank) ? rank : 99)
                                  .FirstOrDefault() ?? string.Empty,

                        ApproverId = g.Select(x => x.ApproverValue)
                                      .Select(v => int.TryParse(v, out var n) ? n : (int?)null)
                                      .Where(n => n.HasValue)
                                      .Select(n => n.Value)
                                      .FirstOrDefault()
                    });

                foreach (var line in Indent.IndentDetails)
                {
                    if (byLine.TryGetValue(line.Id, out var agg))
                    {
                        line.Status = agg.Status;
                        line.ApproverId = agg.ApproverId;   // 0 if none found
                    }
                    else
                    {
                        line.Status = string.Empty;
                        // line.ApproverId stays default
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
                
            }
            
            return Indent;
        }
    }
}