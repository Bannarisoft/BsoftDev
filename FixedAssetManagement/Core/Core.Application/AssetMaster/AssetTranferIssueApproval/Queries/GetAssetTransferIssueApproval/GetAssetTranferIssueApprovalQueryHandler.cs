using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetTransferIssueApproval;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetTranferIssueApproval.Queries.GetAssetTransferIssueApproval
{
    public class GetAssetTranferIssueApprovalQueryHandler : IRequestHandler<GetAssetTranferIssueApprovalQuery,  ApiResponseDTO<List<AssetTransferIssueApprovalDto>>>
    {
        private readonly IAssetTransferIssueApprovalQueryRepository _assetTransferIssueQueryRepository;
        private readonly IMapper _mapper;        
        private readonly IMediator _mediator; 
        public GetAssetTranferIssueApprovalQueryHandler( IAssetTransferIssueApprovalQueryRepository assetTransferIssueQueryRepository, IMapper mapper, IMediator mediator)
        {
            _assetTransferIssueQueryRepository = assetTransferIssueQueryRepository;
             _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<AssetTransferIssueApprovalDto>>> Handle(GetAssetTranferIssueApprovalQuery request, CancellationToken cancellationToken)
        {
           var (assetIssueTransfer, totalCount) = await _assetTransferIssueQueryRepository
                                                .GetAllPendingAssetTransferAsync(request.PageNumber, request.PageSize, request.SearchTerm, request.FromDate, request.ToDate);
            var assetIssueTransferList = _mapper.Map<List<AssetTransferIssueApprovalDto>>(assetIssueTransfer);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "Get",        
                actionName: assetIssueTransferList.Count.ToString(),
                details: $"Asset Transfer Pending details was fetched.",
                module:"Asset Transfer Pending"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<AssetTransferIssueApprovalDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = assetIssueTransfer,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize                
            };   
        }
    }
}