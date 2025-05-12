using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IActivityMaster;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using MassTransit.Mediator;
using MediatR;

namespace Core.Application.ActivityMaster.Queries.GetActivityType
{
    public class GetActivityTypeQueryHandler   : IRequestHandler<GetActivityTypeQuery, ApiResponseDTO<List<GetMiscMasterDto>>>
    {
             private readonly IActivityMasterQueryRepository _activityMasterQueryRepository;
        private readonly IMapper _mapper;
    

         public GetActivityTypeQueryHandler(IActivityMasterQueryRepository activityMasterQueryRepository, IMapper mapper)
        {
            _activityMasterQueryRepository = activityMasterQueryRepository;
            _mapper =mapper;
         
           // _departmentGrpcClient = departmentGrpcClient;

        } 

         public async Task<ApiResponseDTO<List<GetMiscMasterDto>>> Handle(GetActivityTypeQuery request, CancellationToken cancellationToken)
        {
            var DepMethod = await _activityMasterQueryRepository.GetActivityTypeAsync();
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