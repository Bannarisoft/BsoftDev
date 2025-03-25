using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IWorkCenter;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WorkCenter.Queries.GetWorkCenter
{
    public class GetWorkCenterQueryHandler : IRequestHandler<GetWorkCenterQuery,ApiResponseDTO<List<WorkCenterDto>>>
    {
        private readonly IWorkCenterQueryRepository _iWorkCenterQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetWorkCenterQueryHandler(IWorkCenterQueryRepository iWorkCenterQueryRepository, IMapper mapper, IMediator mediator)
        {
            _iWorkCenterQueryRepository = iWorkCenterQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<WorkCenterDto>>> Handle(GetWorkCenterQuery request, CancellationToken cancellationToken)
        {
             var (WorkCenter, totalCount) = await _iWorkCenterQueryRepository.GetAllWorkCenterGroupAsync(request.PageNumber, request.PageSize, request.SearchTerm);
               var workCentersgrouplist = _mapper.Map<List<WorkCenterDto>>(WorkCenter);

             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetWorkCenter",
                    actionCode: "Get",        
                    actionName: WorkCenter.Count().ToString(),
                    details: $"WorkCenter details was fetched.",
                    module:"WorkCenter"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<WorkCenterDto>> 
            { 
                IsSuccess = true, 
                Message = "Success", 
                Data = workCentersgrouplist ,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
                };
        }

    }
}