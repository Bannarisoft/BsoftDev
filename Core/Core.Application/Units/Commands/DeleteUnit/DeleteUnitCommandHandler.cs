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
          private readonly IMapper _Imapper;
           private readonly ILogger<DeleteUnitCommandHandler> _logger;

        public DeleteUnitCommandHandler(IUnitCommandRepository iunitrepository,IMapper Imapper,ILogger<DeleteUnitCommandHandler> logger)
        {
            _iunitRepository = iunitrepository;
            _Imapper = Imapper;
            _logger = logger?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponseDTO<int>> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
        {
              _logger.LogInformation("Starting Deletion process for UnitId: {UnitId}", request.UnitId);
              var unit = _Imapper.Map<Core.Domain.Entities.Unit>(request.UpdateUnitStatusDto);
              var result =await _iunitRepository.DeleteUnitAsync(request.UnitId,unit);
               if (result == -1) // Unit not found
                {   
                    _logger.LogInformation("Unit {UnitId} not found.", request.UnitId);
                    return new ApiResponseDTO<int> { IsSuccess = false, Message = "Unit not found." };
                }
              _logger.LogInformation("Completed Deletion process for UnitId: {UnitId}", request.UnitId);
              var unitId = unit.Id;
              _logger.LogInformation("Unit {UnitId} deleted successfully", unitId);
              return new ApiResponseDTO<int> { 
                IsSuccess = true, 
                Message = "Unit deleted successfully", 
                Data = unitId 
                };
       

        }

    }
}