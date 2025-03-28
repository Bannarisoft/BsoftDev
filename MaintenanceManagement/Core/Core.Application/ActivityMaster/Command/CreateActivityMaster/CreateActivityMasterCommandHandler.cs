using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.ActivityMaster.Queries.GetAllActivityMaster;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IActivityMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.ActivityMaster.Command.CreateActivityMaster
{
    public class CreateActivityMasterCommandHandler  : IRequestHandler<CreateActivityMasterCommand, ApiResponseDTO<GetAllActivityMasterDto>>
    {
        private readonly IActivityMasterCommandRepository _activityMasterCommandRepository;
        private readonly IActivityMasterQueryRepository _activityMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        
        public CreateActivityMasterCommandHandler(
            IActivityMasterCommandRepository activityMasterCommandRepository,
            IActivityMasterQueryRepository activityMasterQueryRepository,
            IMapper mapper,
            IMediator mediator)
        {
            _activityMasterCommandRepository = activityMasterCommandRepository;
            _activityMasterQueryRepository = activityMasterQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<GetAllActivityMasterDto>> Handle(CreateActivityMasterCommand request, CancellationToken cancellationToken)
        {
            // 🔹 Map request to domain entity
            var activityMaster = _mapper.Map<Core.Domain.Entities.ActivityMaster>(request);

            // 🔹 Insert into the database
            var result = await _activityMasterCommandRepository.CreateAsync(activityMaster);

            if (result.Id <= 0)
            {
                return new ApiResponseDTO<GetAllActivityMasterDto>
                {
                    IsSuccess = false,
                    Message = "Failed to create Activity",
                    Data = null
                };
            }

            // 🔹 Fetch newly created record
            var createdActivityMaster = await _activityMasterQueryRepository.GetByIdAsync(result.Id);
            var mappedResult = _mapper.Map<GetAllActivityMasterDto>(createdActivityMaster);

            // 🔹 Publish domain event for auditing/logging
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: activityMaster.ActivityName,
                actionName: "Activity Created",
                details: $"Activity '{activityMaster.ActivityName}' was created.",
                module: "ActivityMaster"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            // 🔹 Return success response
            return new ApiResponseDTO<GetAllActivityMasterDto>
            {
                IsSuccess = true,
                Message = "Activity created successfully",
                Data = mappedResult
            };
        }

    }
}