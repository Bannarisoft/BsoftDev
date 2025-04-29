using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMaintenanceRequest;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using MediatR;

namespace Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequestType
{
    public class GetMaintenanceRequestTypeQueryHandler : IRequestHandler<GetMaintenanceRequestTypeQuery, ApiResponseDTO<List<GetMiscMasterDto>>>
    {

        private readonly IMaintenanceRequestQueryRepository _maintenanceRequestQueryRepository;
        private readonly IMapper _mapper;

         public GetMaintenanceRequestTypeQueryHandler(IMaintenanceRequestQueryRepository maintenanceRequestQueryRepository , IMapper mapper)
        {
            _maintenanceRequestQueryRepository = maintenanceRequestQueryRepository;
            _mapper = mapper;
        }

         public async Task<ApiResponseDTO<List<GetMiscMasterDto>>> Handle(GetMaintenanceRequestTypeQuery request, CancellationToken cancellationToken)
        {
            var DepMethod = await _maintenanceRequestQueryRepository.GetMaintenanceStatusDescAsync();
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