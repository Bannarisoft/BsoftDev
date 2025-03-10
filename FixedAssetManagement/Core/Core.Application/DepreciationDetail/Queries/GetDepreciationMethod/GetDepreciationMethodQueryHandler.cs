using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IDepreciationDetail;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using MediatR;

namespace Core.Application.DepreciationDetail.Queries.GetDepreciationMethod
{
    public class GetDepreciationMethodQueryHandler : IRequestHandler<GetDepreciationMethodQuery, ApiResponseDTO<List<GetMiscMasterDto>>>
    {
        private readonly IDepreciationDetailQueryRepository _repository;
        private readonly IMapper _mapper;

        public GetDepreciationMethodQueryHandler(IDepreciationDetailQueryRepository repository, IMapper mapper)
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