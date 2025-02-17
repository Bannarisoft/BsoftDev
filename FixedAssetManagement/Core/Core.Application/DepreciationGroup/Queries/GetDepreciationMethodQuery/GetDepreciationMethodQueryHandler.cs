using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IDepreciationGroup;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using MediatR;

namespace Core.Application.DepreciationGroup.Queries.GetDepreciationMethodQuery
{
    public class GetDepreciationMethodQueryHandler : IRequestHandler<GetDepreciationMethodQuery, ApiResponseDTO<List<GetMiscMasterDto>>>
    {
        private readonly IDepreciationGroupQueryRepository _repository;
        private readonly IMapper _mapper;

        public GetDepreciationMethodQueryHandler(IDepreciationGroupQueryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ApiResponseDTO<List<GetMiscMasterDto>>> Handle(GetDepreciationMethodQuery request, CancellationToken cancellationToken)
        {
            var DepMethod = await _repository.GetDepreciationMethodAsync();
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