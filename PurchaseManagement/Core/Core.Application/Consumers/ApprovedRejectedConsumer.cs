using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Commands.Purchase;
using Contracts.Commands.Workflow;
using Contracts.Events.Workflow;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IPurchaseIndent;
using Core.Domain.Entities;
using MassTransit;

namespace Core.Application.Consumers
{
    public class ApprovedRejectedConsumer : IConsumer<UpdateIndentDetailCommand>
    {
        private readonly IPurchaseIndentCommand _purchaseIndentCommand;
        private readonly IMapper _imapper;
        private readonly IEventPublisher _eventPublisher;
        public ApprovedRejectedConsumer(IPurchaseIndentCommand purchaseIndentCommand, IMapper mapper, IEventPublisher eventPublisher)
        {
            _purchaseIndentCommand = purchaseIndentCommand;
            _imapper = mapper;
            _eventPublisher = eventPublisher;
        }
        public async Task Consume(ConsumeContext<UpdateIndentDetailCommand> context)
        {
            try
            {
                var Indent = _imapper.Map<List<IndentDetail>>(context.Message.ApprovedQty);
                await _purchaseIndentCommand.UpdateIndentDetailAsync(Indent);

            }
            catch (Exception ex)
            {
                  var correlationId = Guid.NewGuid();
                var @event = new ApprovedRejectedEvent
                {
                    CorrelationId = correlationId,
                    IndentId = context.Message.IndentId,
                    ApprovedQty = context.Message.ApprovedQty
                };
                
                await _eventPublisher.SaveEventAsync(@event);
                await _eventPublisher.PublishPendingEventsAsync();
            }
            
        }
    }
}