using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Commands.Purchase;
using Contracts.Commands.Workflow;
using Contracts.Events.Workflow;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IPurchaseIndent;
using Core.Domain.Common;
using Core.Domain.Entities;
using MassTransit;

namespace Core.Application.Consumers
{
    public class ApprovedRejectedConsumer : IConsumer<UpdateIndentDetailCommand>
    {
        private readonly IPurchaseIndentCommand _purchaseIndentCommand;
        private readonly IMapper _imapper;
        private readonly IEventPublisher _eventPublisher;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        public ApprovedRejectedConsumer(IPurchaseIndentCommand purchaseIndentCommand, IMapper mapper, IEventPublisher eventPublisher,
         IMiscMasterQueryRepository miscMasterQueryRepository)
        {
            _purchaseIndentCommand = purchaseIndentCommand;
            _imapper = mapper;
            _eventPublisher = eventPublisher;
            _miscMasterQueryRepository = miscMasterQueryRepository;
        }
        public async Task Consume(ConsumeContext<UpdateIndentDetailCommand> context)
        {
            try
            {

                var msg = context.Message;
                var status = msg.Status;
                if (status == MiscEnumEntity.Pending)
                {
                    var Indent = _imapper.Map<List<IndentDetail>>(context.Message.ApprovedQty);


                    await _purchaseIndentCommand.UpdateIndentDetailAsync(Indent);
                }

                else if (status == MiscEnumEntity.Approved || status == MiscEnumEntity.Rejected)
                {
                    var Indentheader = _imapper.Map<IndentHeader>(context.Message);
                    var headerStatus = await _miscMasterQueryRepository.GetMiscMasterByName(MiscEnumEntity.Status, status);
                    Indentheader.StatusId = headerStatus.Id;
                    var StatusApproved = await _miscMasterQueryRepository.GetMiscMasterByName(MiscEnumEntity.Status, MiscEnumEntity.Approved);
                    var StatusRejected = await _miscMasterQueryRepository.GetMiscMasterByName(MiscEnumEntity.Status, MiscEnumEntity.Rejected);

                    foreach (var item in Indentheader.IndentDetails)
                    {

                        item.StatusId = item.ApprovedQuantity > 0 ? StatusApproved.Id : StatusRejected.Id;

                    }
                    await _purchaseIndentCommand.FinalizeStatus(Indentheader);
                }
                // else
                // {
                //     throw new InvalidOperationException($"Unknown status '{status}'.");
                // }


            }
            catch (Exception ex)
            {
                // var correlationId = Guid.NewGuid();
                // var @event = new ApprovedRejectedEvent
                // {
                //     CorrelationId = correlationId,
                //     IndentId = context.Message.IndentId,
                //     ApprovedQty = context.Message.ApprovedQty
                // };

                // await _eventPublisher.SaveEventAsync(@event);
                // await _eventPublisher.PublishPendingEventsAsync();
                
                throw;
            }
            
        }
    }
}