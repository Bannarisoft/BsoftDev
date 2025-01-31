using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ITimeZones;
using Core.Application.TimeZones.Queries.GetTimeZones;
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.TimeZones.Queries.GetTimeZonesAutoComplete
{
    public class GetTimeZonesAutocompleteQueryHandler : IRequestHandler<GetTimeZonesAutocompleteQuery, ApiResponseDTO<List<TimeZonesDto>>>
    {
        private readonly ITimeZonesQueryRepository _timeZonesQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        private readonly ILogger<GetTimeZonesAutocompleteQueryHandler> _logger;

        public GetTimeZonesAutocompleteQueryHandler(ITimeZonesQueryRepository timeZonesQueryRepository, IMapper mapper, IMediator mediator, ILogger<GetTimeZonesAutocompleteQueryHandler> logger)
        {
            _timeZonesQueryRepository = timeZonesQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponseDTO<List<TimeZonesDto>>> Handle(GetTimeZonesAutocompleteQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching TimeZones Request started: {request}", request); 
            var newTimeZones = await _timeZonesQueryRepository.GetByTimeZonesNameAsync(request.SearchPattern);
            if (newTimeZones == null || !newTimeZones.Any() || newTimeZones.Count == 0)
            {
                _logger.LogWarning("No TimeZones Record {TimeZones} not found in DB.", newTimeZones.Count);
                return new ApiResponseDTO<List<TimeZonesDto>>
                {
                    IsSuccess = false,
                    Message = "No TimeZones found"
                };
            }
            else
            {
                var newTimeZonesDto = _mapper.Map<List<TimeZonesDto>>(newTimeZones);
                _logger.LogInformation("Fetching TimeZones Request Completed: {request}", newTimeZonesDto.Count);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetTimeZonesAutocompleteQuery",
                    actionCode: "Get TimeZones",                
                    actionName: newTimeZonesDto.Count.ToString(),
                    details: $"TimeZones details was fetched.",
                    module:"TimeZones");
                await _mediator.Publish(domainEvent);
                _logger.LogInformation("TimeZones {TimeZones} Listed successfully.", newTimeZonesDto.Count);
                return new ApiResponseDTO<List<TimeZonesDto>>
                {
                    IsSuccess = true,
                    Message = "Success",
                    Data = newTimeZonesDto
                };
            }           
            
        }
    }
}