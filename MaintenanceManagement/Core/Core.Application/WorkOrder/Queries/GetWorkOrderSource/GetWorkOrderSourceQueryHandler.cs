
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using MediatR;

namespace Core.Application.WorkOrder.Queries.GetWorkOrderSource
{
    public class GetWorkOrderSourceQueryHandler  : IRequestHandler<GetWorkOrderSourceQuery, ApiResponseDTO<List<GetMiscMasterDto>>>
    {
        private readonly IWorkOrderQueryRepository _repository;
        private readonly IMapper _mapper;

        public GetWorkOrderSourceQueryHandler(IWorkOrderQueryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ApiResponseDTO<List<GetMiscMasterDto>>> Handle(GetWorkOrderSourceQuery request, CancellationToken cancellationToken)
        {
            var DepMethod = await _repository.GetWOStatusDescAsync();
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