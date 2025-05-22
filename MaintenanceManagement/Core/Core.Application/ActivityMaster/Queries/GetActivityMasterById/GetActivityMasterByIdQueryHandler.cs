using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IActivityMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineGroup.Queries.GetMachineGroupById
{
    public class GetActivityMasterByIdQueryHandler : IRequestHandler<GetActivityMasterByIdQuery, ApiResponseDTO<GetActivityMasterByIdDto>>
    {

        private readonly IActivityMasterQueryRepository _activityMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetActivityMasterByIdQueryHandler(IActivityMasterQueryRepository activityMasterQueryRepository, IMapper mapper, IMediator mediator)
        {
            _activityMasterQueryRepository = activityMasterQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<GetActivityMasterByIdDto>> Handle(GetActivityMasterByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _activityMasterQueryRepository.GetByIdAsync(request.Id);

            if (result is null)
            {
                return new ApiResponseDTO<GetActivityMasterByIdDto>
                {
                    IsSuccess = false,
                    Message = $"ActivityMaster with Id {request.Id} not found.",
                    Data = null
                };
            }

            var machineGroup = _mapper.Map<GetActivityMasterByIdDto>(result);
            // Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: "",
                actionName: "",
                details: $"ActivityMaster details {machineGroup.Id} were fetched.",
                module: "ActivityMaster"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<GetActivityMasterByIdDto>
            {
                IsSuccess = true,
                Message = "Success",
                Data = machineGroup
            };
        }



    }
}