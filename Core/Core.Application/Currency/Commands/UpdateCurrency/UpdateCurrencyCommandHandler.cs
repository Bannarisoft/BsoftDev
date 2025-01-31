using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ICurrency;
using Core.Domain.Enums;
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Currency.Commands.UpdateCurrency
{
    public class UpdateCurrencyCommandHandler : IRequestHandler<UpdateCurrencyCommand, ApiResponseDTO<int>>
    {
        private readonly ICurrencyCommandRepository _currencyCommandRepository;
        private readonly IMapper _Imapper;
        private readonly ILogger<UpdateCurrencyCommandHandler> _logger;
        private readonly IMediator _mediator; 

        public UpdateCurrencyCommandHandler(ICurrencyCommandRepository currencyCommandRepository,IMapper Imapper,ILogger<UpdateCurrencyCommandHandler> logger,IMediator mediator)
        {
            _currencyCommandRepository = currencyCommandRepository;
            _Imapper = Imapper;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<int>> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Currency Update process for CurrencyId: {CurrencyId}", request.Id);
            var currency = _Imapper.Map<Core.Domain.Entities.Currency>(request);
            var result = await _currencyCommandRepository.UpdateAsync(request.Id, currency);
            if (result == -1) // Currency not found
            {
                _logger.LogInformation("Currency {CurrencyId} not found.", request.Id);
                return new ApiResponseDTO<int> { IsSuccess = false, Message = "Currency not found." };
            }
            
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: currency.Code,
                    actionName: currency.Name,
                    details: $"Currency '{currency.Code}' was Updated. CurrencyCode: {request.Id}",
                    module: "Currency"
                    );    
                await _mediator.Publish(domainEvent, cancellationToken);
                _logger.LogInformation("Currency {Currency} Updated successfully.", currency.Name);
            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "Success",
                Data = result
            };
                       
        }
    }

}