using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptDetailsById;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetTransferReceipt;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetRecieptDtlPending
{
    public class GetAssetRecieptDtlPendingQueryHandler : IRequestHandler<GetAssetRecieptDtlPendingQuery, ApiResponseDTO<List<AssetTransferReceiptDtlPendingDto>>> 
    {
        
        private readonly IAssetTransferReceiptQueryRepository _assetTransferReceiptQueryRepository;
        private readonly IMapper _mapper;        
        private readonly IMediator _mediator; 

        public GetAssetRecieptDtlPendingQueryHandler(IAssetTransferReceiptQueryRepository assetTransferReceiptQueryRepository, IMapper mapper, IMediator mediator)
        {
            _assetTransferReceiptQueryRepository = assetTransferReceiptQueryRepository;
            _mapper = mapper;            
            _mediator = mediator;   
        }

        public async Task<ApiResponseDTO<List<AssetTransferReceiptDtlPendingDto>>> Handle(GetAssetRecieptDtlPendingQuery request, CancellationToken cancellationToken)
        {
        var result = await _assetTransferReceiptQueryRepository.GetAllPendingAssetTransferDtlAsync(request.AssetTransferId);

        // Check if data exists
        if (result is null || !result.Any())
        {
            return new ApiResponseDTO<List<AssetTransferReceiptDtlPendingDto>>
            {
                IsSuccess = false,
                Message = $"No records found for ID {request.AssetTransferId}."
            };
        }

        // Map list of results
        var assetTransferpendingReceiptList = _mapper.Map<List<AssetTransferReceiptDtlPendingDto>>(result);

        // Domain Event Logging
        var domainEvent = new AuditLogsDomainEvent(
            actionDetail: "GetAssetRecieptDtlPendingQuery",
            actionCode: "AssetTransferReceiptPending",
            actionName: request.AssetTransferId.ToString(),
            details: $"Asset transfer Receipt Details Pending details for ID {request.AssetTransferId} were fetched.",
            module: "AssetTransferReceipt"
        );
        await _mediator.Publish(domainEvent, cancellationToken);

        return new ApiResponseDTO<List<AssetTransferReceiptDtlPendingDto>>
        {
            IsSuccess = true,
            Message = "Success",
            Data = assetTransferpendingReceiptList
        };
        }

    }
}