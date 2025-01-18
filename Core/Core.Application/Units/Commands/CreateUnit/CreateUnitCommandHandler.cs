using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Core.Application.Common.Interfaces.IUnit;
using Core.Application.Common;
using Core.Domain.Events;

namespace Core.Application.Units.Commands.CreateUnit
{

    public class CreateUnitCommandHandler : IRequestHandler<CreateUnitCommand, Result<int>>
    {
        
        private readonly IUnitCommandRepository _iUnitRepository;
        private readonly IMapper _mapper;

         private readonly IMediator _Imediator;

        private readonly ILogger<CreateUnitCommandHandler> _logger;
        public CreateUnitCommandHandler(IUnitCommandRepository iUnitRepository, IMapper mapper, ILogger<CreateUnitCommandHandler> logger, IMediator Imediator)
        {
            _iUnitRepository = iUnitRepository;
            _mapper = mapper;
            _logger = logger;
            _Imediator = Imediator;

        }
        public async Task<Result<int>> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
        {
        try
        {
            
        // Map the request to a Unit object
        var unit = _mapper.Map<Core.Domain.Entities.Unit>(request.UnitDto);

        // Create the unit
        await _iUnitRepository.CreateUnitAsync(unit);

        // Get the unit ID
        var unitId = unit.Id;

        foreach (var addressDto in request.UnitDto.UnitAddressDto)
        {
            var address = _mapper.Map<UnitAddress>(addressDto);
            address.UnitId = unitId;

            await _iUnitRepository.CreateUnitAddressAsync(address);
        }

        foreach (var contactDto in request.UnitDto.UnitContactsDto)
        {
            var contact = _mapper.Map<UnitContacts>(contactDto);
            contact.UnitId = unitId;

            await _iUnitRepository.CreateUnitContactsAsync(contact);
        }

         //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: unit.Id.ToString(),
                actionName: unit.UnitName,
                details: $"Entity '{unit.UnitName}' was created. EntityCode: {unit.Id}",
                module:"Unit"
            );
            await _Imediator.Publish(domainEvent, cancellationToken);
            return Result<int>.Success(unitId);

        }
        catch (Exception ex)
        {
        // Log the exception
        _logger.LogError(ex, "Error creating unit");

        // Throw a exception
         throw new Exception("Error creating unit", ex);
        }     
    }            
}

    
}