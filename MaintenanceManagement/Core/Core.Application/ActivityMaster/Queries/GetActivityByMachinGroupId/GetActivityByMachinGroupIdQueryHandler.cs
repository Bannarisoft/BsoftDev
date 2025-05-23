using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IActivityMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.ActivityMaster.Queries.GetActivityByMachinGroupId
{
    public class GetActivityByMachinGroupIdQueryHandler : IRequestHandler<GetActivityByMachinGroupIdQuery, ApiResponseDTO<List<GetActivityByMachinGroupDto>>>
    {

        private readonly IActivityMasterQueryRepository _activityMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetActivityByMachinGroupIdQueryHandler(IActivityMasterQueryRepository activityMasterQueryRepository, IMapper mapper, IMediator mediator)
        {
            _activityMasterQueryRepository = activityMasterQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
        
        public async Task<ApiResponseDTO<List<GetActivityByMachinGroupDto>>> Handle(GetActivityByMachinGroupIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _activityMasterQueryRepository.GetActivityByMachinGroupId(request.MachineGroupId);

            if (result == null || !result.Any()) // Check if the list is empty
            {
                return new ApiResponseDTO<List<GetActivityByMachinGroupDto>>
                {
                    IsSuccess = false,
                    Message = $"Activity Name  for MachineGroup with Id {request.MachineGroupId} not found.",
                    Data = null
                };
            }

            var ActivityList = _mapper.Map<List<GetActivityByMachinGroupDto>>(result); // Map the list

            

            // Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: "",
                actionName: "",
                details: $"Activity Name  for MachineGroupById {request.MachineGroupId} were fetched.",
                module: "Activity Name"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<List<GetActivityByMachinGroupDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = ActivityList
            };
        }


    }
}