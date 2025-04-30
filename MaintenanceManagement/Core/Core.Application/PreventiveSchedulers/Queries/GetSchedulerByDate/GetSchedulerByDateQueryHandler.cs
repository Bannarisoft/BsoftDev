using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Queries.GetSchedulerByDate
{
    public class GetSchedulerByDateQueryHandler : IRequestHandler<GetSchedulerByDateQuery, ApiResponseDTO<List<SchedulerByDateDto>>>
    {
         private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
         private readonly IMapper _mapper;
        
        public GetSchedulerByDateQueryHandler(IPreventiveSchedulerQuery preventiveSchedulerQuery,IMapper mapper)
        {
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
            _mapper = mapper;
        }
        public async Task<ApiResponseDTO<List<SchedulerByDateDto>>> Handle(GetSchedulerByDateQuery request, CancellationToken cancellationToken)
        {
            var preventiveScheduler = await _preventiveSchedulerQuery.GetAbstractSchedulerByDate();
            
            var preventiveSchedulerList = _mapper.Map<List<SchedulerByDateDto>>(preventiveScheduler);

          
            return new ApiResponseDTO<List<SchedulerByDateDto>> 
            { 
                IsSuccess = true, 
                Message = "Success", 
                Data = preventiveSchedulerList 
            };
        }
    }
}