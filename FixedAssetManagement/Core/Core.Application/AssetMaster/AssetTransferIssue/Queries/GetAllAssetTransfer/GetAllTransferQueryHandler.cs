using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAllAssetTransfer
{
    
    public class GetAllTransferQueryHandler : IRequestHandler<GetAllTransferQuery, ApiResponseDTO<List<GetAllTransferDtlDto>>>
    {
        private readonly IAssetTransferQueryRepository _assetTransferQueryRepository; 
        
        public GetAllTransferQueryHandler(IAssetTransferQueryRepository assetTransferQueryRepository)
            {
                _assetTransferQueryRepository = assetTransferQueryRepository;
            }        

      public async Task<ApiResponseDTO<List<GetAllTransferDtlDto>>> Handle(GetAllTransferQuery request, CancellationToken cancellationToken)
        {
            // Fetch asset details from the repository
            var assetTransferHdr = await _assetTransferQueryRepository.GetAssetTransferByIDAsync(request.AssetTransferId);

            if (assetTransferHdr == null || !assetTransferHdr.Any())
            {
                return new ApiResponseDTO<List<GetAllTransferDtlDto>> // Fix: Use List<GetAllTransferHdrDto>
                {
                    IsSuccess = false,
                    Message = "Asset Transfer details not found",
                    Data = new List<GetAllTransferDtlDto>() // Return empty list instead of null
                };
            }

            return new ApiResponseDTO<List<GetAllTransferDtlDto>> // Fix: Use List<GetAllTransferHdrDto>
            {
                IsSuccess = true,
                Message = "Asset Transfer retrieved successfully.",
                Data = assetTransferHdr
            };
        }
    }
}