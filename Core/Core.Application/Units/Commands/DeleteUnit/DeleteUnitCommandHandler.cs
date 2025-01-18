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
           private readonly ILogger<DeleteUnitCommandHandler> _Ilogger;

        public DeleteUnitCommandHandler(IUnitCommandRepository iunitrepository,IMapper Imapper,ILogger<DeleteUnitCommandHandler> Ilogger)
        {
            _iunitRepository = iunitrepository;
            _Imapper = Imapper;
            _Ilogger = Ilogger;
        }

        public async Task<ApiResponseDTO<int>> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
        {
       
              var unit = _Imapper.Map<Core.Domain.Entities.Unit>(request.UpdateUnitStatusDto);
              await _iunitRepository.DeleteUnitAsync(request.UnitId,unit);
              var unitId = unit.Id;
              return new ApiResponseDTO<int> { 
                IsSuccess = true, 
                Message = "Unit deleted successfully", 
                Data = unitId 
                };
       

        }

    }
}