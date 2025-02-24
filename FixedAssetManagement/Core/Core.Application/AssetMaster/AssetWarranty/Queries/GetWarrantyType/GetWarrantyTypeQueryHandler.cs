using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetWarranty;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using MediatR;

namespace Core.Application.AssetMaster.AssetWarranty.Queries.GetWarrantyType
{
    public class GetWarrantyTypeQueryHandler  : IRequestHandler<GetWarrantyTypeQuery, ApiResponseDTO<List<GetMiscMasterDto>>>
    {
        private readonly IAssetWarrantyQueryRepository _repository;
        private readonly IMapper _mapper;

        public GetWarrantyTypeQueryHandler(IAssetWarrantyQueryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ApiResponseDTO<List<GetMiscMasterDto>>> Handle(GetWarrantyTypeQuery request, CancellationToken cancellationToken)
        {
            var warrantyType = await _repository.GetWarrantyTypeAsync();
            var warrantyTypeList = _mapper.Map<List<GetMiscMasterDto>>(warrantyType);

            return new ApiResponseDTO<List<GetMiscMasterDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = warrantyTypeList,
                TotalCount = warrantyTypeList.Count
            };
        }
    }
}