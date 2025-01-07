using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Core.Application.Common.Interfaces.IUnit;

namespace Core.Application.Units.Commands.DeleteUnit
{
    public class DeleteUnitCommandHandler : IRequestHandler<DeleteUnitCommand,int>
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

        public async Task<int> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
        {
        try
        {
        var unit = _Imapper.Map<Core.Domain.Entities.Unit>(request.UpdateUnitStatusDto);
        await _iunitRepository.DeleteUnitAsync(request.UnitId,unit);
        var unitId = unit.Id;
        return unitId;
        }   
        catch (Exception ex)
        {
                // Log the exception
                _Ilogger.LogError(ex, "Error updating unit");

                // Throw a custom exception 
                throw new Exception("Error updating unit", ex);
        }
    }

    }
}