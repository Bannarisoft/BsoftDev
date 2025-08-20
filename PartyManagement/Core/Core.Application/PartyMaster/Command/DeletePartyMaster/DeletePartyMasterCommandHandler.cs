using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Interfaces.IPartyMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.PartyMaster.Command.DeletePartyMaster
{
    public class DeletePartyMasterCommandHandler : IRequestHandler<DeletePartyMasterCommand, bool>
    {

        private readonly IPartyMasterCommandRepository _ipartyMasterCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;

        public DeletePartyMasterCommandHandler(IPartyMasterCommandRepository ipartyMasterCommandRepository, IMediator imediator, IMapper imapper)
        {
            _ipartyMasterCommandRepository = ipartyMasterCommandRepository;
            _imediator = imediator;
            _imapper = imapper;
        }

        public async Task<bool> Handle(DeletePartyMasterCommand request, CancellationToken cancellationToken)
        {
             var partymaster = _imapper.Map<Core.Domain.Entities.PartyMaster>(request);
            var result = await _ipartyMasterCommandRepository.DeleteAsync(request.Id,partymaster);
          
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: partymaster.Id.ToString(),
                actionName: partymaster.PartyCode ?? "NULL",
                details: $"PartyMaster details was deleted",
                module: "PartyMaster");
            await _imediator.Publish(domainEvent);

            return result == true ? result : throw new ExceptionRules("PartyMaster deletion failed.");
        }
    }
}