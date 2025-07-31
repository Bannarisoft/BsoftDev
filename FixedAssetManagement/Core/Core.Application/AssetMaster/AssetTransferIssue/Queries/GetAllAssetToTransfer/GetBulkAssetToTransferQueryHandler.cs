using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetDtlToTransfer;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using FluentValidation;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetBulkAssetToTransfer
{
    public class GetBulkAssetToTransferQueryHandler : IRequestHandler<GetBulkAssetToTransferQuery, List<GetAssetDetailsToTransferHdrDto>>
    {

        private readonly IAssetTransferQueryRepository _assetTransferQueryRepository;
        private readonly IMapper _mapper;
        public GetBulkAssetToTransferQueryHandler(IAssetTransferQueryRepository assetTransferQueryRepository, IMapper mapper)
        {
            _assetTransferQueryRepository = assetTransferQueryRepository;
            _mapper = mapper;
        }


            public async Task<List<GetAssetDetailsToTransferHdrDto>> Handle(GetBulkAssetToTransferQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.CustodianId))
            {
                throw new ValidationException("CustodianId is required.");
              
            }

            var asset = await _assetTransferQueryRepository.GetAssetDetailsToTransferByFiltersAsync(request.CustodianId, request.DepartmentId, request.CategoryID);

            var assetList = _mapper.Map<List<GetAssetDetailsToTransferHdrDto>>(asset);
            
            return assetList;
        }

    }
}