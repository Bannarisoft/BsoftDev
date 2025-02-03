using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ICurrency;
using Core.Application.Currency.Queries.GetCurrency;
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Currency.Queries.GetCurrencyById
{
    public class GetCurrencyByIdQueryHandler : IRequestHandler<GetCurrencyByIdQuery, ApiResponseDTO<List<CurrencyDto>>>
    {
        private readonly ICurrencyQueryRepository _currencyQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<GetCurrencyByIdQueryHandler> _logger;

        public GetCurrencyByIdQueryHandler(ICurrencyQueryRepository currencyQueryRepository, IMapper mapper, IMediator mediator, ILogger<GetCurrencyByIdQueryHandler> logger)
        {
            _currencyQueryRepository = currencyQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponseDTO<List<CurrencyDto>>> Handle(GetCurrencyByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Fetching Currency Request started: {request.CurrencyId}");
            var newcurrency = await _currencyQueryRepository.GetByIdAsync(request.CurrencyId);
            if (newcurrency is null || !newcurrency.Any() || newcurrency.Count == 0)
            {
                _logger.LogWarning($"No Currency Record {newcurrency.Count} not found in DB.");
                return new ApiResponseDTO<List<CurrencyDto>>
                {
                    IsSuccess = false,
                    Message = "No currency found"
                };
            }
            var currencylist = _mapper.Map<List<CurrencyDto>>(newcurrency);
            _logger.LogInformation($"Fetching Currency Request Completed: {currencylist.Count}");
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetCurrencyByIdQuery",
                actionCode: "Get Currency",                
                actionName: currencylist.Count.ToString(),
                details: $"Currency details was fetched.",
                module:"Currency");
            await _mediator.Publish(domainEvent, cancellationToken);
            _logger.LogInformation($"Currency {currencylist.Count} Listed successfully.");
            return new ApiResponseDTO<List<CurrencyDto>>
            {                
                IsSuccess = true,
                Message = "Success",
                Data = currencylist
            };
        }
    }

}