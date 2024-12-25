using BSOFT.Application.Units.Queries.GetUnits;
using BSOFT.Application.Units.Commands.CreateUnit;
using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace BSOFT.Application.Units.Commands.CreateUnit
{

    public class CreateUnitCommandHandler : IRequestHandler<CreateUnitCommand, int>
    {
        
        private readonly IUnitRepository _iUnitRepository;
        private readonly IMapper _mapper;

        private readonly ILogger<CreateUnitCommandHandler> _logger;
        public CreateUnitCommandHandler(IUnitRepository iUnitRepository, IMapper mapper, ILogger<CreateUnitCommandHandler> logger)
        {
            _iUnitRepository = iUnitRepository;
            _mapper = mapper;
            _logger = logger;

        }
        public async Task<int> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
        {
        try
        {
        // Map the request to a Unit object
        var unit = _mapper.Map<BSOFT.Domain.Entities.Unit>(request.UnitDto);

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

        return unitId;



        




        // Map the request to a UnitAddress object
        // var unitAddress = _mapper.Map<UnitAddress>(request.unitDto);

        // // Set the UnitId property of the unitAddress object
        // unitAddress.UnitId = unitId;

        // // Create the unit address
        // await _iUnitRepository.CreateUnitAddressAsync(unitAddress);

        // // Map the request to a UnitContacts object
        // var unitContacts = _mapper.Map<UnitContacts>(request.unitDto);

        // // Set the UnitId property of the unitContacts object
        // unitContacts.UnitId = unitId;

        // // Create the unit contacts
        // await _iUnitRepository.CreateUnitContactsAsync(unitContacts);

        // // Return the unit ID
        // return unitId;
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