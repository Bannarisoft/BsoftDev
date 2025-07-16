using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetCustodian
{
    public class GetAssetCustodianQueryHandler : IRequestHandler<GetAssetCustodianQuery, ApiResponseDTO<List<GetAssetCustodianDto>>>
    {
        private readonly IAssetTransferQueryRepository _assetTransferQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetAssetCustodianQueryHandler(IAssetTransferQueryRepository assetTransferQueryRepository, IMapper mapper, IMediator mediator)
        {
            _assetTransferQueryRepository = assetTransferQueryRepository;
            _mapper = mapper;
            _mediator = mediator;

        }

        public async Task<ApiResponseDTO<List<GetAssetCustodianDto>>> Handle(GetAssetCustodianQuery request, CancellationToken cancellationToken)
        {
            var oldUnitId = request.OldUnitId;
              

                if (string.IsNullOrWhiteSpace(oldUnitId))
                {
                    return new ApiResponseDTO<List<GetAssetCustodianDto>>
                    {
                        IsSuccess = false,
                        Message = "OldUnitId not found in token.",
                        Data = null
                    };
                }
            
              var result = await _assetTransferQueryRepository.GetCustodianByDepartmentAsync(oldUnitId, request.DepartmentId);

                return new ApiResponseDTO<List<GetAssetCustodianDto>>
                {
                    IsSuccess = true,
                    Message = "Success",
                    Data = result
                };
        }
    }
}