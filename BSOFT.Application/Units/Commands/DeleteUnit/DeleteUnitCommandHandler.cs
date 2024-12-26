using BSOFT.Application.Units.Queries.GetUnits;
using BSOFT.Application.Units.Commands.DeleteUnit;
using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using System;
using static BSOFT.Application.Units.Commands.DeleteUnit.DeleteUnitCommand;

namespace BSOFT.Application.Units.Commands.DeleteUnit
{
    public class DeleteUnitCommandHandler : IRequestHandler<DeleteUnitCommand,int>
    {
          private readonly IUnitRepository _iunitRepository;
          private readonly IMapper _mapper;
           private readonly ILogger<DeleteUnitCommandHandler> _logger;

        public DeleteUnitCommandHandler(IUnitRepository iunitRepository,IMapper mapper,ILogger<DeleteUnitCommandHandler> logger)
        {
            _iunitRepository = iunitRepository;
            _mapper = mapper;
            _logger = logger;

            
        }

        public async Task<int> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
        {
        try
        {
        var unit = _mapper.Map<BSOFT.Domain.Entities.Unit>(request.UpdateUnitStatusDto);
        await _iunitRepository.DeleteUnitAsync(request.UnitId,unit);
        var unitId = unit.Id;
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