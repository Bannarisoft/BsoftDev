using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Queries.GetPreventiveSchedulerById
{
    public class GetPreventiveSchedulerByIdQueryHandler : IRequestHandler<GetPreventiveSchedulerByIdQuery, ApiResponseDTO<PreventiveSchedulerHdrByIdDto>>
    {
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetPreventiveSchedulerByIdQueryHandler(IPreventiveSchedulerQuery preventiveSchedulerQuery, IMapper mapper, IMediator mediator)
        {
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<PreventiveSchedulerHdrByIdDto>> Handle(GetPreventiveSchedulerByIdQuery request, CancellationToken cancellationToken)
        {
             var result = await _preventiveSchedulerQuery.GetByIdAsync(request.Id);
            var preventiveScheduler = _mapper.Map<PreventiveSchedulerHdrByIdDto>(result);

          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "",        
                    actionName: "",
                    details: $"PreventiveScheduler details was fetched.",
                    module:"PreventiveScheduler"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return new ApiResponseDTO<PreventiveSchedulerHdrByIdDto> 
          { 
            IsSuccess = true, 
            Message = "Success", 
            Data = preventiveScheduler 
            };
        }
    }
}