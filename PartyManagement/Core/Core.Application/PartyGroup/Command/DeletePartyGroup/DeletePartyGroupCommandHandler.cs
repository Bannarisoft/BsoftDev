using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Interfaces.IPartyGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.PartyGroup.Command.DeletePartyGroup
{
    public class DeletePartyGroupCommandHandler : IRequestHandler<DeletePartyGroupCommand, bool>
    {
        
        private readonly IPartyGroupCommandRepository _ipartygroupcommandrepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        public DeletePartyGroupCommandHandler(IPartyGroupCommandRepository ipartygroupcommandrepository, IMediator imediator, IMapper imapper)
        {
            _ipartygroupcommandrepository = ipartygroupcommandrepository;
            _imediator = imediator;
            _imapper = imapper;
        }
        public async Task<bool> Handle(DeletePartyGroupCommand request, CancellationToken cancellationToken)
        {
            var partyGroupmaster = _imapper.Map<Core.Domain.Entities.PartyGroup>(request);
            var result = await _ipartygroupcommandrepository.DeleteAsync(request.Id,partyGroupmaster);
          
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: partyGroupmaster.Id.ToString(),
                actionName: partyGroupmaster.PartyGroupName ?? "NULL",
                details: $"PartyGroup details was deleted",
                module: "PartyGroup");
            await _imediator.Publish(domainEvent);

            return result == true ? result : throw new ExceptionRules("PartyGroup deletion failed.");
        }
    }
}