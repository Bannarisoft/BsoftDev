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

        private readonly IUnitQueryRepository _iunitQueryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateUnitCommandHandler> _logger;

       public UpdateUnitCommandHandler(IUnitCommandRepository iUnitRepository, IMapper mapper, ILogger<UpdateUnitCommandHandler> logger, IUnitQueryRepository IunitQueryRepository)
        {
            _iUnitRepository = iUnitRepository;
            _mapper = mapper;
            _logger = logger?? throw new ArgumentNullException(nameof(logger));
            _iunitQueryRepository = IunitQueryRepository;
        }

        public async Task<ApiResponseDTO<int>> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
        {
       
            _logger.LogInformation($"Starting update process for UnitId: {request.UpdateUnitDto.Id}");
            //  First, check if the ID exists in the database
            var existingUnit = await _iunitQueryRepository.GetByIdAsync(request.UpdateUnitDto.Id);
            if (existingUnit is null || existingUnit.Count == 0)
            {
                _logger.LogWarning($"Unit ID {request.UpdateUnitDto.Id} not found.");
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Unit Id not found / Unit is deleted."
                };
            }

            // Check if unit name already exists for another ID
            var existingUnitName = await _iUnitRepository.ExistsByNameupdateAsync(request.UpdateUnitDto.UnitName,request.UpdateUnitDto.Id);
            if (existingUnitName)
            {
                _logger.LogWarning($"Unit name {request.UpdateUnitDto.UnitName} already exists.");
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Unit name already exists."
                };
            }

            var unit = _mapper.Map<Core.Domain.Entities.Unit>(request.UpdateUnitDto);
            var result =await _iUnitRepository.UpdateUnitAsync(request.UpdateUnitDto.Id, unit);
            if (result == -1)
            {
                 _logger.LogWarning($"UnitId not found: {request.UpdateUnitDto.Id}");

                    // The unit was not found, 
                    return new ApiResponseDTO<int>
                  {
                      IsSuccess = false,
                      Message = "UnitId not found",
                  };
           
            }
            _logger.LogInformation($"Completed update process for UnitId: {request.UpdateUnitDto.Id}");

              var unitId = unit.Id;
              _logger.LogInformation($"Unit {unitId} Fetched successfully For Other Tables UnitAddress and UnitContacts");

              _logger.LogInformation($"Unit {unitId} Updated successfully");
              return new ApiResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Unit updated successfully",
                    Data = unitId
                };

        
        }

       
    }
}