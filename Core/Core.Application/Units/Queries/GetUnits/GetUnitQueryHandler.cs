using AutoMapper;
using Core.Application.Common;

using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IUnit;
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Core.Application.Units.Queries.GetUnits
{
    public class GetUnitQueryHandler : IRequestHandler<GetUnitQuery,ApiResponseDTO<List<UnitDto>>>
    {
        private readonly IUnitQueryRepository _unitRepository;        
        private readonly IMapper _mapper;

        private readonly IMediator _mediator;
         private readonly ILogger<GetUnitQueryHandler> _logger;
        public GetUnitQueryHandler( IUnitQueryRepository unitRepository, IMapper mapper, IMediator mediator,ILogger<GetUnitQueryHandler> logger)
        {
            _unitRepository = unitRepository;
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponseDTO<List<UnitDto>>> Handle(GetUnitQuery request, CancellationToken cancellationToken)
        {
        
             _logger.LogInformation("Fetching Unit Request started: {request}", request);
            var units = await _unitRepository.GetAllUnitsAsync();
             if (units == null || !units.Any() || units.Count == 0)
                {
                 _logger.LogWarning("No Unit Record {Unit} not found in DB.", units.Count);
                     return new ApiResponseDTO<List<UnitDto>>
                     {
                         IsSuccess = false,
                         Message = "No Unit found"
                     };
                }
            var unitList = _mapper.Map<List<UnitDto>>(units);
            _logger.LogInformation("Fetching Unit Request Completed: {request}", units.Count);
            //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetUnitQuery",
                    actionCode: "Get Units",        
                    actionName: "Get",
                    details: $"Units details was fetched.",
                    module:"Unit"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
                _logger.LogInformation("Unit {Unit} Listed successfully.", units.Count);
                return new ApiResponseDTO<List<UnitDto>>
                {
                    IsSuccess = true,
                    Message = "Success",
                    Data = unitList
                };
           
        
        }
    }
}