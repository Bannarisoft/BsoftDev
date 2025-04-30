using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Queries.GetDetailSchedulerByDate
{
    public class GetDetailSchedulerByDateQueryHandler : IRequestHandler<GetDetailSchedulerByDateQuery, ApiResponseDTO<List<DetailSchedulerByDateDto>>>
    {
         private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
         private readonly IMapper _mapper;
        public GetDetailSchedulerByDateQueryHandler(IPreventiveSchedulerQuery preventiveSchedulerQuery,IMapper mapper)
        {
             _preventiveSchedulerQuery = preventiveSchedulerQuery;
            _mapper = mapper;
        }
        public async Task<ApiResponseDTO<List<DetailSchedulerByDateDto>>> Handle(GetDetailSchedulerByDateQuery request, CancellationToken cancellationToken)
        {
             var preventiveScheduler = await _preventiveSchedulerQuery.GetDetailSchedulerByDate(request.SchedulerDate);
            var preventiveSchedulerList = _mapper.Map<List<DetailSchedulerByDateDto>>(preventiveScheduler);

          
            return new ApiResponseDTO<List<DetailSchedulerByDateDto>> 
            { 
                IsSuccess = true, 
                Message = "Success", 
                Data = preventiveSchedulerList 
            };
        }
    }
}