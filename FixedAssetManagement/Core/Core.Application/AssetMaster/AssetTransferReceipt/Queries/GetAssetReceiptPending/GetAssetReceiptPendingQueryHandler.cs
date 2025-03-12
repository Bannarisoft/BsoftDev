using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetTransferReceipt;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptPending
{
    public class GetAssetReceiptPendingQueryHandler : IRequestHandler<GetAssetReceiptPendingQuery,  ApiResponseDTO<List<AssetTransferReceiptPendingDto>>>
    {
        private readonly IAssetTransferReceiptQueryRepository _assetTransferReceiptQueryRepository;
        private readonly IMapper _mapper;        
        private readonly IMediator _mediator; 

        public GetAssetReceiptPendingQueryHandler(IAssetTransferReceiptQueryRepository assetTransferReceiptQueryRepository, IMapper mapper, IMediator mediator)
        {
            _assetTransferReceiptQueryRepository = assetTransferReceiptQueryRepository;
            _mapper = mapper;            
            _mediator = mediator;   
        }

        public async Task<ApiResponseDTO<List<AssetTransferReceiptPendingDto>>> Handle(GetAssetReceiptPendingQuery request, CancellationToken cancellationToken)
        {
             var (assetIssueTransfer, totalCount) = await _assetTransferReceiptQueryRepository
                                                .GetAllPendingAssetTransferAsync(request.PageNumber, request.PageSize, request.SearchTerm, request.FromDate, request.ToDate);
            var assetIssueTransferList = _mapper.Map<List<AssetTransferReceiptPendingDto>>(assetIssueTransfer);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "Get",        
                actionName: assetIssueTransferList.Count.ToString(),
                details: $"Asset Receipt Pending details was fetched.",
                module:"Asset Receipt Pending"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<AssetTransferReceiptPendingDto>>
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