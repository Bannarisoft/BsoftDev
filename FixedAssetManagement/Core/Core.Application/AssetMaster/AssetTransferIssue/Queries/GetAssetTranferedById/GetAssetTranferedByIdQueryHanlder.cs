using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTranferedById
{
   public class  GetAssetTranferedByIdQueryHanlder  : IRequestHandler<GetAssetTranferedByIdQuery, ApiResponseDTO<AssetTransferJsonDto>>

    {
       private readonly IAssetTransferQueryRepository _assetTransferQueryRepository;       


       // ✅ Constructor
        public GetAssetTranferedByIdQueryHanlder ( IAssetTransferQueryRepository assetTransferQueryRepository)
        {
            _assetTransferQueryRepository = assetTransferQueryRepository;
        }
         // ✅ Handle Method
        public async Task<ApiResponseDTO<AssetTransferJsonDto>> Handle(GetAssetTranferedByIdQuery request, CancellationToken cancellationToken)
        {
            var assetTransfer = await _assetTransferQueryRepository.GetAssetTransferByIdAsync(request.AssetTransferId);

            if (assetTransfer == null)
            {
                return new ApiResponseDTO<AssetTransferJsonDto>
                {
                    IsSuccess = false,
                    Message = $"Asset Transfer Issue with ID {request.AssetTransferId} not found."
                
                };
            }
                return new ApiResponseDTO<AssetTransferJsonDto>
                {
                    IsSuccess = true,
                    Message = "Asset Transfer retrieved successfully.",
                    Data = assetTransfer
                };            
        }
    }
}

