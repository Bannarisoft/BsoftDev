using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Events.Workflow;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.ILogService;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IPurchaseIndent;
using Core.Application.PurchaseIndents.Command.CreatePurchaseIndent;
using Core.Domain.Common;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.PurchaseIndents.Command.UpdatePurchaseIndent
{
    public class UpdatePurchaseIndentCommandHandler : IRequestHandler<UpdatePurchaseIndentCommand, bool>
    {
        private readonly IPurchaseIndentCommand _purchaseIndentCommand;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPurchaseIndentQuery _purchaseIndentQuery;
        public UpdatePurchaseIndentCommandHandler(IPurchaseIndentCommand purchaseIndentCommand, IMediator imediator, IMapper imapper,
        IEventPublisher eventPublisher, IPurchaseIndentQuery purchaseIndentQuery)
        {
            _purchaseIndentCommand = purchaseIndentCommand;
            _imediator = imediator;
            _imapper = imapper;
            _eventPublisher = eventPublisher;
            _purchaseIndentQuery = purchaseIndentQuery;
        }
        public async Task<bool> Handle(UpdatePurchaseIndentCommand request, CancellationToken cancellationToken)
        {
            var Indent = _imapper.Map<IndentHeader>(request);
            
            var result = await _purchaseIndentCommand.UpdateAsync(Indent,JsonSerializer.Serialize(request));

            var indentData = await _purchaseIndentQuery.GetByIdAsync(request.Id);
            
            var indentReverseMap = _imapper.Map<IndentReverseMapDto>(indentData);

            string serializedPayload = JsonSerializer.Serialize(indentReverseMap);

             if (result && request.IsDraft == 0)
            {
                var correlationId = Guid.NewGuid();
                var @event = new TransactionCreatedEvent
                {
                    CorrelationId = correlationId,
                    ModuleTypeName = MiscEnumEntity.PurchaseIndent,
                    ModuleTransactionId = request.Id,
                    Payload = serializedPayload
                };

                await _eventPublisher.SaveEventAsync(@event);
                await _eventPublisher.PublishPendingEventsAsync();
            }
            return result == true ? result : throw new ExceptionRules("Indent update failed."); 
        }
    }
}