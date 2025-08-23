using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IPartyMaster;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.PartyMaster.Command.DeletePartyMaster
{
    public class DeletePartyMasterCommandHandler : IRequestHandler<DeletePartyMasterCommand, bool>
    {

        private readonly IPartyMasterCommandRepository _ipartyMasterCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        private readonly IPartyActivityLogCommandRepository _ipartyActivityLogCommandRepository;
        
        private readonly IIPAddressService _ipAddressService;

        public DeletePartyMasterCommandHandler(IPartyMasterCommandRepository ipartyMasterCommandRepository, IMediator imediator, IMapper imapper, IPartyActivityLogCommandRepository ipartyActivityLogCommandRepository, IIPAddressService ipAddressService)
        {
            _ipartyMasterCommandRepository = ipartyMasterCommandRepository;
            _imediator = imediator;
            _imapper = imapper;
            _ipartyActivityLogCommandRepository = ipartyActivityLogCommandRepository;
            _ipAddressService = ipAddressService;
          
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

            if (result)
            {
                await _ipartyMasterCommandRepository.LogChange(partymaster.Id, "PartyMaster", "IsDeleted", "0", "1", "Delete");  

                return true;
            }
            else
            {
                throw new ExceptionRules("PartyMaster deletion failed.");
            }
        }
    }
}