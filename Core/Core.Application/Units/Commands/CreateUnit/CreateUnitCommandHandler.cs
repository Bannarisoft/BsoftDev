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
       
            
            _logger.LogInformation("Starting creation process for Unit: {UnitName}", request.UnitDto.UnitName);
              var unit = _mapper.Map<Core.Domain.Entities.Unit>(request.UnitDto);
              var result =  await _iUnitRepository.CreateUnitAsync(unit);
            _logger.LogInformation("Completed creation process for Unit: {UnitName}", request.UnitDto.UnitName);
            

              var unitId = unit.Id;
              _logger.LogInformation("Unit {UnitId} created successfully", unitId);

                foreach (var addressDto in request.UnitDto.UnitAddressDto)
              {
                 _logger.LogInformation("Starting creation process for UnitAddress: {UnitId}", unitId);
                  var address = _mapper.Map<UnitAddress>(addressDto);
                  address.UnitId = unitId;
                  await _iUnitRepository.CreateUnitAddressAsync(address);
                  _logger.LogInformation("Completed creation process for UnitAddress: {UnitId}", unitId);
              }

                foreach (var contactDto in request.UnitDto.UnitContactsDto)
                {   
                _logger.LogInformation("Starting creation process for UnitContacts: {UnitId}", unitId);
                  var contact = _mapper.Map<UnitContacts>(contactDto);
                  contact.UnitId = unitId;
                  await _iUnitRepository.CreateUnitContactsAsync(contact);
                 _logger.LogInformation("Completed creation process for UnitContacts: {UnitId}", unitId);
                }

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
                     _logger.LogInformation("Unit {UnitId} created successfully", unitId);
                        return new ApiResponseDTO<int>
                       {
                           IsSuccess = true,
                           Message = "Unit created successfully",
                           Data = unitId
                      };
                 }
                 _logger.LogWarning("Unit {UnitId} Creation Failed", unitId);
                  return new ApiResponseDTO<int>
                  {
                      IsSuccess = false,
                      Message = "Unit not created"
                  };
        }
            

          
    }            
}

    
