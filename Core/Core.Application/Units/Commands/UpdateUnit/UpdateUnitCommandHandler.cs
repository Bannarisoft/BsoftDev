using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Core.Application.Common.Interfaces.IUnit;
using Core.Application.Common.HttpResponse;

namespace Core.Application.Units.Commands.UpdateUnit
{
    public class UpdateUnitCommandHandler : IRequestHandler<UpdateUnitCommand, ApiResponseDTO<int>>
    {
        private readonly IUnitCommandRepository _iUnitRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateUnitCommandHandler> _logger;

       public UpdateUnitCommandHandler(IUnitCommandRepository iUnitRepository, IMapper mapper, ILogger<UpdateUnitCommandHandler> logger)
        {
            _iUnitRepository = iUnitRepository;
            _mapper = mapper;
            _logger = logger?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponseDTO<int>> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
        {
       
            _logger.LogInformation("Starting update process for UnitId: {UnitId}", request.UpdateUnitDto.Id);
            var unit = _mapper.Map<Core.Domain.Entities.Unit>(request.UpdateUnitDto);
            var result =await _iUnitRepository.UpdateUnitAsync(request.UnitId, unit);
            if (result == -1)
            {
                 _logger.LogWarning("UnitId not found: {UnitId}", request.UpdateUnitDto.Id);

                    // The unit was not found, 
                    return new ApiResponseDTO<int>
                  {
                      IsSuccess = false,
                      Message = "UnitId not found",
                  };
           
            }
            _logger.LogInformation("Completed update process for UnitId: {UnitId}", request.UpdateUnitDto.Id);

              var unitId = unit.Id;
              _logger.LogInformation("Unit {UnitId} Fetched successfully For Other Tables UnitAddress and UnitContacts", unitId);

              foreach (var addressDto in request.UpdateUnitDto.UnitAddressDto)
              {
                 _logger.LogInformation("Starting update process for  UnitAddress: {UnitId}", unitId);
                  var address = _mapper.Map<UnitAddress>(addressDto);
                  address.UnitId = unitId;
                  await _iUnitRepository.UpdateUnitAddressAsync(request.UnitId, address);
                  _logger.LogInformation("Completed update process for  UnitAddress: {UnitId}", unitId);
              }

              foreach (var contactDto in request.UpdateUnitDto.UnitContactsDto)
              { 
                 _logger.LogInformation("Starting update process for  UnitContacts: {UnitId}", unitId);
                  var contact = _mapper.Map<UnitContacts>(contactDto);
                  contact.UnitId = unitId;
                  await _iUnitRepository.UpdateUnitContactsAsync(request.UnitId, contact);
                  _logger.LogInformation("Completed update process for  UnitContacts: {UnitId}", unitId);
              }
              _logger.LogInformation("Unit {UnitId} Updated successfully", unitId);
              return new ApiResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Unit updated successfully",
                    Data = unitId
                };

        
        }

       
    }
}