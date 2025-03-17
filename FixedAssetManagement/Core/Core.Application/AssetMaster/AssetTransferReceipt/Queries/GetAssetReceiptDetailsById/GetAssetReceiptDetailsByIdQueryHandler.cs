using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetTransferReceipt;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptDetailsById
{
    public class GetAssetReceiptDetailsByIdQueryHandler : IRequestHandler<GetAssetReceiptDetailsByIdQuery, ApiResponseDTO<List<AssetReceiptDetailsByIdDto>>> 
    {
         private readonly IAssetTransferReceiptQueryRepository _assetTransferReceiptQueryRepository;
        private readonly IMapper _mapper;        
        private readonly IMediator _mediator; 

        public GetAssetReceiptDetailsByIdQueryHandler(IAssetTransferReceiptQueryRepository assetTransferReceiptQueryRepository, IMapper mapper, IMediator mediator)
        {
            _assetTransferReceiptQueryRepository = assetTransferReceiptQueryRepository;
            _mapper = mapper;            
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<AssetReceiptDetailsByIdDto>>> Handle(GetAssetReceiptDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _assetTransferReceiptQueryRepository.GetByAssetReceiptId(request.AssetReceiptId);

        // Check if data exists
        if (result is null || !result.Any())
        {
            return new ApiResponseDTO<List<AssetReceiptDetailsByIdDto>>
            {
                IsSuccess = false,
                Message = $"No records found for ID {request.AssetReceiptId}."
            };
        }

        // Map list of results
        var assetTransferReceiptList = _mapper.Map<List<AssetReceiptDetailsByIdDto>>(result);

        // Domain Event Logging
        var domainEvent = new AuditLogsDomainEvent(
            actionDetail: "GetById",
            actionCode: "AssetTransferReceipt",
            actionName: request.AssetReceiptId.ToString(),
            details: $"Asset transfer Receipt details for ID {request.AssetReceiptId} were fetched.",
            module: "AssetTransferReceipt"
        );
        await _mediator.Publish(domainEvent, cancellationToken);

        return new ApiResponseDTO<List<AssetReceiptDetailsByIdDto>>
        {
            IsSuccess = true,
            Message = "Success",
            Data = assetTransferReceiptList
        };
        }
    }
}