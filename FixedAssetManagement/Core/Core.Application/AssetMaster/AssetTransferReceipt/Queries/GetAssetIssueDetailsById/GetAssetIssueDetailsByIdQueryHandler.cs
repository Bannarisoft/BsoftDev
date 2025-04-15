using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using Core.Application.Common.Interfaces.IAssetTransferReceipt;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetIssueDetailsById
{
    public class GetAssetIssueDetailsByIdQueryHandler : IRequestHandler<GetAssetIssueDetailsByIdQuery, ApiResponseDTO<AssetTransferJsonDto>>
    {
        private readonly IAssetTransferReceiptQueryRepository _assetTransferQueryRepository;  
        public GetAssetIssueDetailsByIdQueryHandler ( IAssetTransferReceiptQueryRepository assetTransferQueryRepository)
        {
            _assetTransferQueryRepository = assetTransferQueryRepository;
        }

        public Task<ApiResponseDTO<AssetTransferJsonDto>> Handle(GetAssetIssueDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        // public async Task<ApiResponseDTO<AssetTransferJsonDto>> Handle(GetAssetIssueDetailsByIdQuery request, CancellationToken cancellationToken)
        // {
        //     var assetTransfer = await _assetTransferQueryRepository.GetAssetTransferByIdAsync(request.AssetTransferId);

        //     if (assetTransfer == null)
        //     {
        //         return new ApiResponseDTO<AssetTransferJsonDto>
        //         {
        //             IsSuccess = false,
        //             Message = $"Asset Transfer Issue with ID {request.AssetTransferId} not found."

        //         };
        //     }
        //         return new ApiResponseDTO<AssetTransferJsonDto>
        //         {
        //             IsSuccess = true,
        //             Message = "Asset Transfer retrieved successfully.",
        //             Data = assetTransfer
        //         };    
        // }
    }
}