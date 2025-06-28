using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetDtlToTransfer;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetBulkAssetToTransfer
{
    public class GetBulkAssetToTransferQueryHandler : IRequestHandler<GetBulkAssetToTransferQuery, ApiResponseDTO<List<GetAssetDetailsToTransferHdrDto>>>
    {

        private readonly IAssetTransferQueryRepository _assetTransferQueryRepository;
        public GetBulkAssetToTransferQueryHandler(IAssetTransferQueryRepository assetTransferQueryRepository)
        {
            _assetTransferQueryRepository = assetTransferQueryRepository;
        }

            public async Task<ApiResponseDTO<List<GetAssetDetailsToTransferHdrDto>>> Handle(GetBulkAssetToTransferQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.CustodianId))
            {
                return new ApiResponseDTO<List<GetAssetDetailsToTransferHdrDto>>
                {
                    IsSuccess = false,
                    Message = "CustodianId is required.",
                    Data = null
                };
            }

            var result = await _assetTransferQueryRepository.GetAssetDetailsToTransferByFiltersAsync(request.CustodianId, request.DepartmentId, request.CategoryID);

            return new ApiResponseDTO<List<GetAssetDetailsToTransferHdrDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = result
            };
        }

    }
}