using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTranferedById;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetDtlToTransfer
{
    public class GetAssetDetailsToTransferQueryHandler  : IRequestHandler<GetAssetDetailsToTransferQuery, ApiResponseDTO<GetAssetDetailsToTransferHdrDto>>
    {

      private readonly IAssetTransferQueryRepository _assetTransferQueryRepository;   

      public GetAssetDetailsToTransferQueryHandler ( IAssetTransferQueryRepository assetTransferQueryRepository)
        {
            _assetTransferQueryRepository = assetTransferQueryRepository;
        }
         public async Task<ApiResponseDTO<GetAssetDetailsToTransferHdrDto>> Handle(GetAssetDetailsToTransferQuery request, CancellationToken cancellationToken)
        {
            // Fetch asset details from the repository
            var assetTransferHdr = await _assetTransferQueryRepository.GetAssetDetailsToTransferByIdAsync(request.AssetId);

            if (assetTransferHdr == null)
            {
                return new ApiResponseDTO<GetAssetDetailsToTransferHdrDto>
                {
                    IsSuccess = false,
                    Message = $"Asset Transfer details not found for Asset ID {request.AssetId}.",
                    Data = null
                };
            }

            return new ApiResponseDTO<GetAssetDetailsToTransferHdrDto>
            {
                IsSuccess = true,
                Message = "Asset Transfer retrieved successfully.",
                Data = assetTransferHdr
            };
        }
        // public async Task<ApiResponseDTO<GetAssetDetailsToTransferHdrDto>> Handle(GetAssetDetailsToTransferQuery request, CancellationToken cancellationToken)
        // {
        //      var assetTransferDtl = await _assetTransferQueryRepository.GetAssetDetailsToTransferByIdAsync(request.AssetId);

        // return assetTransferDtl == null
        //     ? new ApiResponseDTO<GetAssetDetailsToTransferHdrDto> 
        //     { 
        //         IsSuccess = false, 
        //         Message = $"Asset Transfer Issue with ID {request.AssetId} not found." 
        //     }
        //     : new ApiResponseDTO<GetAssetDetailsToTransferHdrDto>
        //     {
        //         IsSuccess = true,
        //         Message = "Asset Transfer retrieved successfully."
                
        //     };
            // var assetTransferdtl = await _assetTransferQueryRepository.GetAssetDetailsToTransferByIdAsync(request.AssetId);

            // if (assetTransferdtl == null)
            // {
            //     return new ApiResponseDTO<GetAssetDetailsToTransferDto>
            //     {
            //         IsSuccess = false,
            //         Message = $"Asset Transfer Issue with ID {request.AssetId} not found."
                
            //     };
            // }
            //     return new ApiResponseDTO<GetAssetDetailsToTransferDto>
            //     {
            //         IsSuccess = true,
            //         Message = "Asset Transfer retrieved successfully.",
            //         Data = assetTransferdtl
            //     };
        // }
    }
}