using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using Core.Domain.Common;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetCodePattern
{
    public class GetAssetCodePAtternQueryHandler : IRequestHandler<GetAssetCodePatternQuery, ApiResponseDTO<List<GetMiscMasterDto>>>
    {
        private readonly IAssetMasterGeneralQueryRepository _repository;
        private readonly IMapper _mapper;

        public GetAssetCodePAtternQueryHandler(IAssetMasterGeneralQueryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ApiResponseDTO<List<GetMiscMasterDto>>> Handle(GetAssetCodePatternQuery request, CancellationToken cancellationToken)
        {
            var assetCode = await _repository.GetAssetPattern();
            var assetCodeList = _mapper.Map<List<GetMiscMasterDto>>(assetCode);
            var assetCodePattern = MiscEnumEntity.Asset_CodePattern.MiscCode;

            // Add AssetCodePattern to each item in the list
            assetCodeList.ForEach(x => x.Code = assetCodePattern);
            return new ApiResponseDTO<List<GetMiscMasterDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = assetCodeList,
                TotalCount = assetCodeList.Count
            };
        }
    }
}