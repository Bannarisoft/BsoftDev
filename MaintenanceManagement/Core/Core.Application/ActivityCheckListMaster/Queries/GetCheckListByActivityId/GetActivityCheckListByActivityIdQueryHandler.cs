using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IActivityCheckListMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.ActivityCheckListMaster.Queries.GetCheckListByActivityId
{
  
     public class GetActivityCheckListByActivityIdQueryHandler : IRequestHandler<GetActivityCheckListByActivityIdQuery, ApiResponseDTO<List<GetActivityCheckListByActivityIdDto>>>
    {
        private readonly IActivityCheckListMasterQueryRepository _activityCheckListMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetActivityCheckListByActivityIdQueryHandler(
            IActivityCheckListMasterQueryRepository activityCheckListMasterQueryRepository,
            IMapper mapper,
            IMediator mediator)
        {
            _activityCheckListMasterQueryRepository = activityCheckListMasterQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<GetActivityCheckListByActivityIdDto>>> Handle(GetActivityCheckListByActivityIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _activityCheckListMasterQueryRepository.GetCheckListByActivityIdsAsync(request.Ids);

            if (result == null || !result.Any())
            {
                return new ApiResponseDTO<List<GetActivityCheckListByActivityIdDto>>
                {
                    IsSuccess = false,
                    Message = $"No activity checklists found for ActivityIds: {string.Join(", ", request.Ids)}.",
                    Data = null
                };
            }

            var checklistDtos = _mapper.Map<List<GetActivityCheckListByActivityIdDto>>(result);

               var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetByActivityId",
                actionCode: string.Join(", ", request.Ids),  // Ensure activity IDs are joined as a string
                actionName: "ActivityCheckList",
                details: $"Fetched {checklistDtos.Count} checklist(s) for ActivityIds: {string.Join(", ", request.Ids)}.", // Better message formatting
                module: "ActivityCheckListMaster"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<List<GetActivityCheckListByActivityIdDto>>
            {
                IsSuccess = true,
                Message = "Activity checklist(s) fetched successfully.",
                Data = checklistDtos
            };
        }
    }
}