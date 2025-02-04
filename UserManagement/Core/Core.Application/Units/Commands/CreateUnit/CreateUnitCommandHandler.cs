using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Core.Application.Common.Interfaces.IUnit;
using Core.Application.Common;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;

namespace Core.Application.Units.Commands.CreateUnit
{

    public class CreateUnitCommandHandler : IRequestHandler<CreateUnitCommand, ApiResponseDTO<int>>
    {
        
        private readonly IUnitCommandRepository _iUnitRepository;
        private readonly IMapper _mapper;

         private readonly IMediator _Imediator;

        private readonly ILogger<CreateUnitCommandHandler> _logger;
        public CreateUnitCommandHandler(IUnitCommandRepository iUnitRepository, IMapper mapper, ILogger<CreateUnitCommandHandler> logger, IMediator Imediator)
        {
            _iUnitRepository = iUnitRepository;
            _mapper = mapper;
            _logger = logger?? throw new ArgumentNullException(nameof(logger));
            _Imediator = Imediator;

        }
        public async Task<ApiResponseDTO<int>> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
        {
       
            
            _logger.LogInformation($"Starting creation process for Unit: {request.UnitName}");
             // Check if Unit Name already exists
            var exists = await _iUnitRepository.ExistsByCodeAsync(request.UnitName);
            if (exists)
            {
                 _logger.LogWarning($"Unit Name {request.UnitName} already exists.");
                 return new ApiResponseDTO<int>
            {
            IsSuccess = false,
            Message = "Unit Name already exists."
        
            };
            }
              var unit = _mapper.Map<Core.Domain.Entities.Unit>(request);
              var result =  await _iUnitRepository.CreateUnitAsync(unit);
            _logger.LogInformation($"Completed creation process for Unit: {request.UnitName}");
            

              var unitId = unit.Id;
              _logger.LogInformation($"Unit {unitId} created successfully");

                   //Domain Event
                  var domainEvent = new AuditLogsDomainEvent(
                      actionDetail: "Create",
                      actionCode: unit.Id.ToString(),
                      actionName: unit.UnitName,
                      details: $"Unit '{unit.UnitName}' was created. UnitId: {unit.Id}",
                      module:"Unit"
                  );
                  await _Imediator.Publish(domainEvent, cancellationToken);
                  if (result > 0)
                  {
                     _logger.LogInformation($"Unit {unitId} created successfully");
                        return new ApiResponseDTO<int>
                       {
                           IsSuccess = true,
                           Message = "Unit created successfully",
                           Data = unitId
                      };
                 }
                 _logger.LogWarning($"Unit {unitId} Creation Failed" );
                  return new ApiResponseDTO<int>
                  {
                      IsSuccess = false,
                      Message = "Unit not created"
                  };
        }
            

          
    }            
}

    
