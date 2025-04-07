using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.ActivityCheckListMaster.Queries.GetActivityCheckListMaster;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IActivityCheckListMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.ActivityCheckListMaster.Queries.GetActivityCheckListMasterById
{
    public class GetActivityCheckListMasterByIdQueryHandler : IRequestHandler<GetActivityCheckListMasterByIdQuery, ApiResponseDTO<GetAllActivityCheckListMasterDto>>
    {

          private readonly IActivityCheckListMasterQueryRepository _activityCheckListMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetActivityCheckListMasterByIdQueryHandler( IActivityCheckListMasterQueryRepository activityCheckListMasterQueryRepository, IMapper mapper, IMediator mediator)
        {
            _activityCheckListMasterQueryRepository = activityCheckListMasterQueryRepository;
            _mapper = mapper;
            _mediator = mediator;  
        }

       public async Task<ApiResponseDTO<GetAllActivityCheckListMasterDto>> Handle(GetActivityCheckListMasterByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _activityCheckListMasterQueryRepository.GetByIdAsync(request.Id);
            
            if (result is null)
            {
                return new ApiResponseDTO<GetAllActivityCheckListMasterDto>
                {
                    IsSuccess = false,
                    Message = $"Activity Checklist with Id {request.Id} not found.",
                    Data = null
                };
            }
            
            var activityChecklist = _mapper.Map<GetAllActivityCheckListMasterDto>(result);

            // Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: "",
                actionName: "",
                details: $"Activity Checklist details {activityChecklist.ChecklistId} were fetched.",
                module: "ActivityCheckListMaster"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<GetAllActivityCheckListMasterDto>
            {
                IsSuccess = true,
                Message = "Success",
                Data = activityChecklist
            };
        }

    }
}