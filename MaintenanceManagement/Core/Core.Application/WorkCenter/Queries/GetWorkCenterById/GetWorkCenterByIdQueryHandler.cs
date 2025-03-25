using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IWorkCenter;
using Core.Application.WorkCenter.Queries.GetWorkCenter;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WorkCenter.Queries.GetWorkCenterById
{
    public class GetWorkCenterByIdQueryHandler : IRequestHandler<GetWorkCenterByIdQuery,ApiResponseDTO<WorkCenterDto>>
    {
      
        private readonly IWorkCenterQueryRepository _iWorkCenterQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;  

        public GetWorkCenterByIdQueryHandler(IWorkCenterQueryRepository iWorkCenterQueryRepository, IMapper mapper, IMediator mediator)
        {
            _iWorkCenterQueryRepository = iWorkCenterQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        } 

        public async Task<ApiResponseDTO<WorkCenterDto>> Handle(GetWorkCenterByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _iWorkCenterQueryRepository.GetByIdAsync(request.Id);
            // Check if the entity exists
            if (result is null)
            {
                return new ApiResponseDTO<WorkCenterDto> { IsSuccess = false, Message =$"WorkCenter ID {request.Id} not found." };
            }
            // Map a single entity
            var workCenter = _mapper.Map<WorkCenterDto>(result);

          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "GetWorkCenterByIdQuery",        
                    actionName: workCenter.Id.ToString(),
                    details: $"WorkCenter details {workCenter.Id} was fetched.",
                    module:"WorkCenter"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return new ApiResponseDTO<WorkCenterDto> { IsSuccess = true, Message = "Success", Data = workCenter };
        }
    }
}