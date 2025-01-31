using MediatR;
using Core.Application.Units.Queries.GetUnits;
using System.Data;
using Core.Application.Common.Interfaces.IUnit;
using AutoMapper;
using Core.Application.Common;
using Core.Domain.Events;

using Core.Application.Common.HttpResponse;
using Microsoft.Extensions.Logging;

namespace Core.Application.Units.Queries.GetUnitAutoComplete
{
    public class GetUnitAutoCompleteQueryHandler : IRequestHandler<GetUnitAutoCompleteQuery, ApiResponseDTO<List<GetUnitsDTO>>>
    {
         private readonly IUnitQueryRepository _unitRepository;        
        private readonly IMapper _mapper;

        private readonly IMediator _mediator; 

        private readonly ILogger<GetUnitAutoCompleteQueryHandler> _logger;


        public GetUnitAutoCompleteQueryHandler(IUnitQueryRepository unitRepository, IMapper mapper, IMediator mediator,ILogger<GetUnitAutoCompleteQueryHandler> logger)
        {
             _unitRepository = unitRepository;
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponseDTO<List<GetUnitsDTO>>> Handle(GetUnitAutoCompleteQuery request, CancellationToken cancellationToken)
        {     
           _logger.LogInformation("Search pattern started: {SearchPattern}", request.SearchPattern);
            var result = await _unitRepository.GetUnit(request.SearchPattern);
              if (result == null || !result.Any() || result.Count == 0) 
                {
                      _logger.LogWarning("No Unit Record {Unit} not found in DB.", request.SearchPattern);
                     return new ApiResponseDTO<List<GetUnitsDTO>>
                     {
                         IsSuccess = false,
                         Message = "Unit not found."
                     };
                }

            var unitDto = _mapper.Map<List<GetUnitsDTO>>(result);

            //Domain Event            
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetUnitAutoCompleteQuery",
                actionCode:"",        
                actionName: request.SearchPattern,                
                details: $"Unit '{request.SearchPattern}' was searched",
                module:"Unit"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            _logger.LogInformation("Unit {Unit} Listed successfully.", result.Count);
            return new ApiResponseDTO<List<GetUnitsDTO>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = unitDto
            };                                    
        }
    }
}