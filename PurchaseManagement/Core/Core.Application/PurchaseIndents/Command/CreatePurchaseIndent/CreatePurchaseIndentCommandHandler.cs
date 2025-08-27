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
using Core.Domain.Common;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.PurchaseIndents.Command.CreatePurchaseIndent
{
    public class CreatePurchaseIndentCommandHandler : IRequestHandler<CreatePurchaseIndentCommand, int>
    {
        private readonly IMapper _imapper;
        private readonly IMediator _mediator;
        private readonly IPurchaseIndentCommand _purchaseIndentCommand;
        private readonly ILogServiceCommand _logServiceCommand;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        private readonly IPurchaseIndentQuery _purchaseIndentQuery;
        private readonly IEventPublisher _eventPublisher;
        public CreatePurchaseIndentCommandHandler(IPurchaseIndentCommand purchaseIndentCommand, IMapper imapper,
        IMediator mediator, ILogServiceCommand logServiceCommand, IMiscMasterQueryRepository miscMasterQueryRepository, IPurchaseIndentQuery purchaseIndentQuery,
        IEventPublisher eventPublisher)
        {
            _purchaseIndentCommand = purchaseIndentCommand;
            _imapper = imapper;
            _mediator = mediator;
            _logServiceCommand = logServiceCommand;
            _miscMasterQueryRepository = miscMasterQueryRepository;
            _purchaseIndentQuery = purchaseIndentQuery;
            _eventPublisher = eventPublisher;
        }
        public async Task<int> Handle(CreatePurchaseIndentCommand request, CancellationToken cancellationToken)
        {
            var Indent = _imapper.Map<IndentHeader>(request);

            var IndentNumber = await _purchaseIndentQuery.GeneratePurchaseIndentNumberAsync(request.UnitId);
            Indent.IndentNumber = IndentNumber;

             var StatusMisc = await _miscMasterQueryRepository.GetMiscMasterByName(MiscEnumEntity.Status, MiscEnumEntity.Draft);
            var StatusPending = await _miscMasterQueryRepository.GetMiscMasterByName(MiscEnumEntity.Status, MiscEnumEntity.Pending);

            Indent.StatusId = request.IsDraft == 1 ? StatusMisc.Id : StatusPending.Id;

            foreach (var item in Indent.IndentDetails)
            {
                item.StatusId = request.IsDraft == 1 ? StatusMisc.Id : StatusPending.Id;
            }
            
            var result = await _purchaseIndentCommand.CreateAsync(Indent);

           

            var indentReverseMap = _imapper.Map<IndentReverseMapDto>(result);

            string serializedPayload = JsonSerializer.Serialize(indentReverseMap);

            var IndentLog = new IndentLog
            {
                IndentHeaderId = result.Id,
                ActionType = "Created",
                ActionRemarks = "Indent Created",
                NewData = serializedPayload,
                StatusId = request.IsDraft == 1 ? StatusMisc.Id : StatusPending.Id
            };

                await _logServiceCommand.CreateAsync(IndentLog);

            if (result.Id > 0 && request.IsDraft ==0)
            {
                var correlationId = Guid.NewGuid();
                var @event = new TransactionCreatedEvent
                {
                    CorrelationId = correlationId,
                    ModuleTypeName = MiscEnumEntity.PurchaseIndent,
                    ModuleTransactionId = result.Id,
                    Payload = serializedPayload
                };

                await _eventPublisher.SaveEventAsync(@event);
                await _eventPublisher.PublishPendingEventsAsync();
            }
            
            return result.Id > 0 ? result.Id : throw new ExceptionRules("Indent Creation Failed.");
        }
    }
}