using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Queries.GetPreventiveScheduler
{
    public class PreventiveSchedulerQueryHandler : IRequestHandler<GetPreventiveSchedulerQuery, ApiResponseDTO<List<GetPreventiveSchedulerDto>>>
    {
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public PreventiveSchedulerQueryHandler(IPreventiveSchedulerQuery preventiveSchedulerQuery, IMapper mapper, IMediator mediator)
        {
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<List<GetPreventiveSchedulerDto>>> Handle(GetPreventiveSchedulerQuery request, CancellationToken cancellationToken)
        {
            var (preventiveScheduler, totalCount) = await _preventiveSchedulerQuery.GetAllPreventiveSchedulerAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var preventiveSchedulerList = _mapper.Map<List<GetPreventiveSchedulerDto>>(preventiveScheduler);

             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetPreventiveScheduler",
                    actionCode: "",        
                    actionName: "",
                    details: $"PreventiveScheduler details was fetched.",
                    module:"PreventiveScheduler"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<GetPreventiveSchedulerDto>> 
            { 
                IsSuccess = true, 
                Message = "Success", 
                Data = preventiveSchedulerList ,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
                };
        }
    }
}