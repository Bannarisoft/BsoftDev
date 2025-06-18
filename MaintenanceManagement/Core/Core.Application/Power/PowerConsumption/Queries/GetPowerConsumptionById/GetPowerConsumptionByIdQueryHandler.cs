using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.Power.IPowerConsumption;
using Core.Application.Power.PowerConsumption.Queries.GetPowerConsumption;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Power.PowerConsumption.Queries.GetPowerConsumptionById
{
    public class GetPowerConsumptionByIdQueryHandler : IRequestHandler<GetPowerConsumptionByIdQuery, ApiResponseDTO<GetPowerConsumptionDto>>
    {
        private readonly IPowerConsumptionQueryRepository _powerConsumptionQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetPowerConsumptionByIdQueryHandler(IPowerConsumptionQueryRepository powerConsumptionQueryRepository, IMapper mapper, IMediator mediator)
        {
            _powerConsumptionQueryRepository = powerConsumptionQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<GetPowerConsumptionDto>> Handle(GetPowerConsumptionByIdQuery request, CancellationToken cancellationToken)
        {
              var result = await _powerConsumptionQueryRepository.GetPowerConsumptionById(request.Id);

            if (result == null)
            {
                return new ApiResponseDTO<GetPowerConsumptionDto>
                {
                    IsSuccess = false,
                    Message = "Id not found.",
                    Data = null
                };
            }

            var feederGroupDto = _mapper.Map<GetPowerConsumptionDto>(result);

            // Domain Event: Audit Logging
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: "POWERCONSUMPTION_VIEW",
                actionName: "View PowerConsumption",
                details: $"PowerConsumption details fetched for Id: {request.Id}",
                module: "Power"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<GetPowerConsumptionDto>
            {
                IsSuccess = true,
                Message = "Success",
                Data = feederGroupDto
            };
        }
    }
}