using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetAdditionalCost;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using MediatR;

namespace Core.Application.AssetMaster.AssetAdditionalCost.Queries.GetCostTypeQuery
{
    public class GetCostTypeQueryHandler : IRequestHandler<GetCostTypeQuery, ApiResponseDTO<List<GetMiscMasterDto>>>
    {
        private readonly IAssetAdditionalCostQueryRepository _iAssetAdditionalCostQueryRepository;
        private readonly IMapper _mapper;

        public GetCostTypeQueryHandler(IAssetAdditionalCostQueryRepository iAssetAdditionalCostQueryRepository, IMapper mapper)
        {
            _iAssetAdditionalCostQueryRepository = iAssetAdditionalCostQueryRepository;
            _mapper = mapper;
        }
        public async Task<ApiResponseDTO<List<GetMiscMasterDto>>> Handle(GetCostTypeQuery request, CancellationToken cancellationToken)
        {
            var costTypes = await _iAssetAdditionalCostQueryRepository.GetCostTypeAsync();
            var costTypeDtoList = _mapper.Map<List<GetMiscMasterDto>>(costTypes);

            return new ApiResponseDTO<List<GetMiscMasterDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = costTypeDtoList,
                TotalCount = costTypeDtoList.Count
            };
        }
    }
}
