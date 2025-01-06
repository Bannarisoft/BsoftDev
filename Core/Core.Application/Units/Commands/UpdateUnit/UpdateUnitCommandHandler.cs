using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Core.Application.Common.Interfaces.IUnit;

namespace Core.Application.Units.Commands.UpdateUnit
{
    public class UpdateUnitCommandHandler : IRequestHandler<UpdateUnitCommand, int>
    {
        private readonly IUnitCommandRepository _iUnitRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateUnitCommandHandler> _logger;

       public UpdateUnitCommandHandler(IUnitCommandRepository iUnitRepository, IMapper mapper, ILogger<UpdateUnitCommandHandler> logger)
        {
            _iUnitRepository = iUnitRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
        {
        try
        {

        var unit = _mapper.Map<Core.Domain.Entities.Unit>(request.UpdateUnitDto);

        // Create the unit
        await _iUnitRepository.UpdateUnitAsync(request.UnitId, unit);

        // Get the unit ID
        var unitId = unit.Id;

        foreach (var addressDto in request.UpdateUnitDto.UnitAddressDto)
        {
            var address = _mapper.Map<UnitAddress>(addressDto);
            address.UnitId = unitId;

            await _iUnitRepository.UpdateUnitAddressAsync(request.UnitId, address);
        }

        foreach (var contactDto in request.UpdateUnitDto.UnitContactsDto)
        {
            var contact = _mapper.Map<UnitContacts>(contactDto);
            contact.UnitId = unitId;

            await _iUnitRepository.UpdateUnitContactsAsync(request.UnitId, contact);
        }

        return unitId;
                
        }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error updating unit");

                // Throw a custom exception
                throw new Exception("Error updating unit", ex);
            }
        }

       
    }
}