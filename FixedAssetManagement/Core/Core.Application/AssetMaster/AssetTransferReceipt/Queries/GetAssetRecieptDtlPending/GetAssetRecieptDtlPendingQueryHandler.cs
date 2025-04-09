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
    public class GetAssetRecieptDtlPendingQueryHandler : IRequestHandler<GetAssetRecieptDtlPendingQuery, ApiResponseDTO<AssetTrasnferReceiptHdrPendingDto>>
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

        public async Task<ApiResponseDTO<AssetTrasnferReceiptHdrPendingDto>> Handle(GetAssetRecieptDtlPendingQuery request, CancellationToken cancellationToken)
        {
             var assetTransfer = await _assetTransferReceiptQueryRepository.GetAssetTransferByIdAsync(request.AssetTransferId);

            if (assetTransfer == null)
            {
                return new ApiResponseDTO<AssetTrasnferReceiptHdrPendingDto>
                {
                    IsSuccess = false,
                    Message = $"Asset Transfer Issue with ID {request.AssetTransferId} not found."
                
                };
            }
                return new ApiResponseDTO<AssetTrasnferReceiptHdrPendingDto>
                {
                    IsSuccess = true,
                    Message = "Asset Transfer retrieved successfully.",
                    Data = assetTransfer
                };   
        }
    }
}