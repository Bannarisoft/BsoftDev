using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.Power.IPowerConsumption;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Power.PowerConsumption.Command.CreatePowerConsumption
{
    public class CreatePowerConsumptionCommandHandler : IRequestHandler<CreatePowerConsumptionCommand, ApiResponseDTO<int>>
    {
        private readonly IPowerConsumptionCommandRepository _powerConsumptionCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;

        public CreatePowerConsumptionCommandHandler(IPowerConsumptionCommandRepository powerConsumptionCommandRepository, IMediator imediator, IMapper imapper)
        {
            _powerConsumptionCommandRepository = powerConsumptionCommandRepository;
            _imediator = imediator;
            _imapper = imapper;
        }

        public async Task<ApiResponseDTO<int>> Handle(CreatePowerConsumptionCommand request, CancellationToken cancellationToken)
        {
              // Calculate TotalUnits
            var totalUnits = request.ClosingReading - request.OpeningReading;
            var powerConsumption = _imapper.Map<Core.Domain.Entities.Power.PowerConsumption>(request);
            powerConsumption.TotalUnits = totalUnits;
            
            var result = await _powerConsumptionCommandRepository.CreateAsync(powerConsumption);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: powerConsumption.FeederTypeId.ToString(),
                actionName: powerConsumption.FeederId.ToString(),
                details: $"PowerConsumption details was created",
                module: "Power");
            await _imediator.Publish(domainEvent, cancellationToken);
          
            var powerConsumptionDto = _imapper.Map<PowerConsumptionDto>(powerConsumption);
            if (result > 0)
                  {
                    
                        return new ApiResponseDTO<int>
                       {
                           IsSuccess = true,
                           Message = "PowerConsumption created successfully",
                           Data = result
                      };
                 }
            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "PowerConsumption Creation Failed",
                Data = result
            };
        }
    }
}