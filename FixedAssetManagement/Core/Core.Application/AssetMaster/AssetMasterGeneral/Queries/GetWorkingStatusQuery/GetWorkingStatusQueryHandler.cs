using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using MediatR;

namespace Core.Application.DepreciationGroup.Queries.GetWorkingStatusQuery
{
    public class GetWorkingStatusQueryHandler : IRequestHandler<GetWorkingStatusQuery, ApiResponseDTO<List<GetMiscMasterDto>>>
    {
        private readonly IAssetMasterGeneralQueryRepository _repository;
        private readonly IMapper _mapper;

        public GetWorkingStatusQueryHandler(IAssetMasterGeneralQueryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ApiResponseDTO<List<GetMiscMasterDto>>> Handle(GetWorkingStatusQuery request, CancellationToken cancellationToken)
        {
            var DepMethod = await _repository.GetWorkingStatusAsync();
            var DepMethodDtoList = _mapper.Map<List<GetMiscMasterDto>>(DepMethod);

            return new ApiResponseDTO<List<GetMiscMasterDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = DepMethodDtoList,
                TotalCount = DepMethodDtoList.Count
            };
        }
    }
}