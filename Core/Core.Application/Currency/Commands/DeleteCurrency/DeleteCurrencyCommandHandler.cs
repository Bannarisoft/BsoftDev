using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ICurrency;
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Currency.Commands.DeleteCurrency
{
    public class DeleteCurrencyCommandHandler : IRequestHandler<DeleteCurrencyCommand, ApiResponseDTO<bool>>
    {
        private readonly ICurrencyCommandRepository _currencyCommandRepository ;

        private readonly ICurrencyQueryRepository currencyQueryRepository;
        private readonly IMapper _Imapper;
        private readonly IMediator _Imediator;
        private readonly ILogger<DeleteCurrencyCommandHandler> _logger;

        public DeleteCurrencyCommandHandler(ICurrencyCommandRepository currencyCommandRepository,IMapper Imapper,IMediator Imediator,ILogger<DeleteCurrencyCommandHandler> logger,ICurrencyQueryRepository currencyQueryRepository)
        {
            _currencyCommandRepository = currencyCommandRepository;
            _Imapper = Imapper;
            _Imediator = Imediator;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.currencyQueryRepository = currencyQueryRepository;
        }

        public async Task<ApiResponseDTO<bool>> Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
        {
         _logger.LogInformation($"Soft Deleting Currency with ID: {request.Id}");

         var currency = await currencyQueryRepository.GetByIdAsync(request.Id);

         if (currency == null)
        {
             _logger.LogWarning($"Soft Deleting Currency Failed with ID: {request.Id}");
            return new ApiResponseDTO<bool>
            {
                IsSuccess = false,
                Message = "Currency not found.",
                Data = false
            };
        }

        var result = await _currencyCommandRepository.SoftDeleteAsync(request.Id);

        if (result)
        {
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Delete",
                    actionCode: request.Id.ToString(),
                    actionName: "DeleteCurrencyCommand",
                    details: $"Currency '{request.Id}' was Deleted.",
                    module: "Currency"
                    );    
                await _Imediator.Publish(domainEvent, cancellationToken);
             _logger.LogInformation($"Soft Deleting Currency Successfully Completed with ID: {request.Id}");
            return new ApiResponseDTO<bool>
            {
                IsSuccess = true,
                Message = "Currency successfully soft deleted.",
                Data = result
            };
            
        }        
        _logger.LogWarning($"Soft Deleting Currency Failed with ID: {request.Id}");
        return new ApiResponseDTO<bool>
        {
            IsSuccess = false,
            Message = "Currency not found.",
            Data = result
        };
        }
    }
}