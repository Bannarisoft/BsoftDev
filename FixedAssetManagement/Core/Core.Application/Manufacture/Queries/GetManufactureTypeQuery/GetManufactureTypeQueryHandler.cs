using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IManufacture;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using MediatR;

namespace Core.Application.DepreciationGroup.Queries.GetManufactureTypeQuery
{
    public class GetManufactureTypeQueryHandler : IRequestHandler<GetManufactureTypeQuery, ApiResponseDTO<List<GetMiscMasterDto>>>
    {
        private readonly IManufactureQueryRepository _repository;
        private readonly IMapper _mapper;

        public GetManufactureTypeQueryHandler(IManufactureQueryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ApiResponseDTO<List<GetMiscMasterDto>>> Handle(GetManufactureTypeQuery request, CancellationToken cancellationToken)
        {
            var DepMethod = await _repository.GetManufactureTypeAsync();
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