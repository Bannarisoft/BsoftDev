using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Contracts.Interfaces.External.IWorkflow;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IPurchaseIndent;
using Core.Domain.Common;
using MediatR;

namespace Core.Application.PurchaseIndents.Queries.GetPendingIndent
{
    public class GetPendingIndentQueryHandler : IRequestHandler<GetPendingIndentQuery, ApiResponseDTO<List<PendingIndentDto>>>
    {
        private readonly IPurchaseIndentQuery _purchaseIndentQuery;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IUnitGrpcClient _unitGrpcClient;
        private readonly IWorkflowGrpcClient _workflowGrpcClient;
        public GetPendingIndentQueryHandler(IPurchaseIndentQuery purchaseIndentQuery, IMediator mediator, IMapper mapper, IUnitGrpcClient unitGrpcClient,
            IWorkflowGrpcClient workflowGrpcClient)
        {
            _purchaseIndentQuery = purchaseIndentQuery;
            _mediator = mediator;
            _mapper = mapper;
            _unitGrpcClient = unitGrpcClient;
            _workflowGrpcClient = workflowGrpcClient;
        }
          public async Task<ApiResponseDTO<List<PendingIndentDto>>> Handle(GetPendingIndentQuery request, CancellationToken cancellationToken)
        {


            var (Indent, TotalCount) = await _purchaseIndentQuery.GetPendingPurchaseIndentAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var IndentDto = _mapper.Map<List<PendingIndentDto>>(Indent);

            var Units = await _unitGrpcClient.GetAllUnitAsync();
            var UnitLookup = Units.ToDictionary(d => d.UnitId, d => d.UnitName);

            foreach (var dto in IndentDto)
            {
                if (UnitLookup.TryGetValue(dto.UnitId, out var UnitName))
                {
                    dto.UnitName = UnitName;
                }
            }

            var FilteredIndent = IndentDto
        .Where(p => UnitLookup.ContainsKey(p.UnitId))
        .ToList();

            // var workflowResponse = await _workflowGrpcClient.GetAllApprovalRequestStatusAsync(MiscEnumEntity.PurchaseIndent);
            // var workflowLookup = workflowResponse.ToDictionary(d => d.ModuleTransactionId, d => d.CurrentStatus);

            // foreach (var statusMap in FilteredIndent)
            // {
            //     if (workflowLookup.TryGetValue(statusMap.Id, out var Status))
            //     {
            //         statusMap.Status = Status;
            //     }
            // }

        //     var FilteredIndentByPending = FilteredIndent
        // .Where(p => workflowLookup.ContainsKey(p.Id))
        // .ToList();

            return new ApiResponseDTO<List<PendingIndentDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = FilteredIndent ?? new List<PendingIndentDto>(),
                TotalCount = TotalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}