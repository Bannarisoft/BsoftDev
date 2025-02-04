using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Core.Application.Common.Interfaces.IUnit;
using Core.Application.Common.HttpResponse;

namespace Core.Application.Units.Commands.DeleteUnit
{
    public class DeleteUnitCommandHandler : IRequestHandler<DeleteUnitCommand,ApiResponseDTO<int>>
    {
          private readonly IUnitCommandRepository _iunitRepository;
          private readonly IUnitQueryRepository _IunitQueryRepository;
          private readonly IMapper _Imapper;
           private readonly ILogger<DeleteUnitCommandHandler> _logger;

        public DeleteUnitCommandHandler(IUnitCommandRepository iunitrepository,IMapper Imapper,ILogger<DeleteUnitCommandHandler> logger,IUnitQueryRepository IunitQueryRepository)
        {
            _iunitRepository = iunitrepository;
            _Imapper = Imapper;
            _logger = logger?? throw new ArgumentNullException(nameof(logger));
            _IunitQueryRepository = IunitQueryRepository;
        }

        public async Task<ApiResponseDTO<int>> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
        {
              _logger.LogInformation($"Starting Deletion process for UnitId: {request.UnitId}");
              // 🔹 First, check if the ID exists in the database
            var existingunitId = await _IunitQueryRepository.GetByIdAsync(request.UnitId);
            if (existingunitId is null || existingunitId.Count == 0)
            {
                _logger.LogWarning($"Unit ID {request.UnitId} not found.");
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Unit Id not found / Unit is deleted ."
                };
            }
              var unit = _Imapper.Map<Core.Domain.Entities.Unit>(request);
              var result =await _iunitRepository.DeleteUnitAsync(request.UnitId,unit);
               if (result == -1) // Unit not found
                {   
                    _logger.LogInformation($"Unit {request.UnitId} not found.");
                    return new ApiResponseDTO<int> { IsSuccess = false, Message = "Unit not found." };
                }
              _logger.LogInformation($"Completed Deletion process for UnitId: {request.UnitId}");
              var unitId = unit.Id;
              _logger.LogInformation($"Unit {unitId} deleted successfully", unitId);
              return new ApiResponseDTO<int> { 
                IsSuccess = true, 
                Message = "Unit deleted successfully", 
                Data = unitId 
                };
       
        }

    }
}